# N1che.Api

Public API service for **N1che** — the newsprint/zine shop-discovery app. Serves niches, shops, routes,
reviews, votes, and bookmarks to the iOS client, and proxies Google Places search/photos and Google
Routes computation so vendor API keys stay server-side. ASP.NET Core (minimal APIs) on .NET 10, backed by
PostgreSQL + PostGIS.

See [`docs/N1che-Api.md`](../docs/N1che-Api.md) for the full API contract, schema, and index policy,
and [`docs/Exceptions.md`](../docs/Exceptions.md) for the domain-exception convention.

---

## Stack

| Concern | Choice |
|---|---|
| Framework | ASP.NET Core minimal APIs (.NET 10) |
| Data access | Dapper over Npgsql (no EF) |
| Database | PostgreSQL 16 + PostGIS 3.4 (geography for radius queries) |
| Migrations | [goose](https://github.com/pressly/goose), run out-of-process |
| Auth | AWS Cognito access tokens (JWT bearer) |
| Validation | FluentValidation via an endpoint filter |
| Third-party | Google Places API (New) + Google Routes API, typed `HttpClient`s with standard resilience |
| API docs | OpenAPI + [Scalar](https://github.com/scalar/scalar) UI (dev only) |
| Health checks | `AspNetCore.HealthChecks.NpgSql` |
| Tests | LightBDD (xUnit) + Testcontainers (PostGIS) + WireMock — service tests per endpoint |
| CI | GitHub Actions — format check + service tests |

---

## Solution layout

```
N1che.slnx
├── src
│   ├── N1che.Api                    # Minimal-API host: Program.cs, endpoints, auth, validation, OpenAPI, exception handling, health
│   ├── N1che.Domain                 # Interfaces, domain services, models, exceptions, Google Places/Routes clients
│   ├── N1che.Contracts              # Request/response DTOs and query filters (wire shapes)
│   └── N1che.Persistence.Postgres   # Npgsql/Dapper readers, writers, transaction scope, goose migrations
└── tests
    └── N1che.ServiceTests           # LightBDD + Testcontainers + WireMock service tests
```

**Conventions:** MVVM-equivalent layering — endpoints are thin, domain services hold logic, persistence
owns all SQL. One type per file. Readers/writers resolve their connection/transaction from the ambient
`ITransactionScope` (`PostgresTransactionScope`). Errors surface as domain exceptions mapped to
`ProblemDetails` by `GlobalExceptionHandler`.

### Geospatial

`shops.location` / `routes.anchor` are `geography(Point,4326)`. Coordinates are kept **SQL-side**: reads
project `ST_Y(location) AS latitude, ST_X(location) AS longitude`, writes build points with
`ST_MakePoint(@lng,@lat)::geography`, and radius filters use `ST_DWithin(...)`. C# only ever sees `double`
lat/lng — no `NetTopologySuite` mapping.

---

## Endpoints

Everything except `/health` requires a Cognito access token (`Authorization: Bearer <token>`). The full
request/response contract is in `docs/N1che-Api.md` and the Scalar UI.

| Method | Route | Summary |
|---|---|---|
| `GET` | `/niches` | Get Niches |
| `GET` | `/shops` | Get Shops (paged) |
| `GET` | `/shops/nearby` | Get Nearby Shops |
| `GET` | `/shops/{id}` | Get Shop By Id |
| `POST` | `/shops` | Add Shop |
| `GET` | `/shops/{id}/interactions` | Get Shop Interactions (vote count + caller's vote/bookmark state) |
| `POST` / `DELETE` | `/shops/{id}/vote` | Add / Remove Shop Vote |
| `POST` / `DELETE` | `/shops/{id}/save` | Save / Unsave Shop |
| `GET` | `/shops/{id}/reviews` | Get Shop Reviews (paged) |
| `POST` | `/shops/{id}/reviews` | Add Shop Review |
| `PUT` | `/shops/{shopId}/reviews/{reviewId}` | Update Shop Review |
| `GET` | `/routes` | Get Routes (top-rated nearby) |
| `GET` | `/routes/{id}` | Get Route By Id |
| `POST` | `/routes` | Save Route |
| `POST` | `/routes/compute` | Compute Route (Google Routes) |
| `GET` | `/routes/{id}/interactions` | Get Route Interactions |
| `POST` / `DELETE` | `/routes/{id}/vote` | Add / Remove Route Vote |
| `GET` | `/places/search` | Search Places (Google Places, with photo attributions) |
| `GET` | `/places/{placeId}/photos/{photoId}/media` | Get Place Photo Media |
| `GET` | `/health/live` | Liveness |
| `GET` | `/health/ready` | Readiness |

---

## Prerequisites

- **.NET 10 SDK** — if installed under `~/.dotnet` and not on `PATH`, export it first:
  ```bash
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$PATH"
  ```
- **Docker** — for the local Postgres/PostGIS instance and Testcontainers.
- **goose** — `brew install goose` (used to apply migrations).

---

## Configuration

`src/N1che.Api/appsettings.json` declares every section with empty values; supply real values through
`appsettings.Development.json`, user secrets, or environment variables (e.g. `GooglePlaces__ApiKey`).
Every section is required — startup throws if one is missing.

| Section | Keys | Purpose |
|---|---|---|
| `Postgres` | `ConnectionString` | Database connection (Development points at the local compose DB) |
| `Cognito` | `Region`, `UserPoolId`, `ClientId` | Issuer and app client used to validate access tokens |
| `GooglePlaces` | `BaseUrl`, `ApiKey` | Places API (New) — search and photo media |
| `GoogleRoutes` | `BaseUrl`, `ApiKey` | Routes API — walking route computation |

---

## Running locally

Start the database (PostGIS), then the API:

```bash
docker compose up -d          # postgis/postgis:16-3.4 on localhost:5432 (db/user/pass all "n1che")
dotnet run --project src/N1che.Api --launch-profile http
```

> `docker-compose.yml` is gitignored — it's a local-dev convenience, not shipped config.

The API listens on **http://localhost:5180** and opens the Scalar UI on launch:

| Route | Purpose |
|---|---|
| `/docs` | Scalar API reference UI (dev only) |
| `/openapi/v1.json` | OpenAPI document (dev only) |
| `/health/live` | Liveness — `200` while the process is up (no dependency checks) |
| `/health/ready` | Readiness — `200` when Postgres is reachable, `503` Problem Details when not |

---

## Migrations

DDL lives as goose migrations in `src/N1che.Persistence.Postgres/Migrations/`, applied out-of-process against
the running database:

```bash
goose -dir src/N1che.Persistence.Postgres/Migrations \
  postgres "host=localhost port=5432 user=n1che password=n1che dbname=n1che sslmode=disable" up
```

The API service owns the schema; the ingestion worker (separate repo) only reads/writes agreed `shops`
columns and never migrates.

---

## Testing

Service tests boot the real API with `WebApplicationFactory` against a PostGIS container (Testcontainers)
that the migrations are applied to — no shared/local DB required. Google Places and Routes are mocked
with WireMock, and requests carry locally-signed Cognito-shaped JWTs so the real bearer pipeline runs.

```bash
dotnet test
```

Every endpoint must have a passing service test before it is considered complete.

---

## CI

`.github/workflows/ci.yml` runs on pull requests and pushes to `main`:

- **format** — `dotnet format N1che.slnx --verify-no-changes`
- **service-tests** — Release build + `dotnet test` (Testcontainers on the runner's Docker); TRX results
  are uploaded on failure

Run `dotnet format N1che.slnx` before pushing to keep the format job green.
