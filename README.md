# N1che.Api

Public API service for **N1che** — the newsprint/zine shop-discovery app. Serves shops, routes,
reviews, votes, and bookmarks to the iOS client. ASP.NET Core (minimal APIs) on .NET 10, backed by
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
| API docs | OpenAPI + [Scalar](https://github.com/scalar/scalar) UI (dev only) |
| Health checks | `AspNetCore.HealthChecks.NpgSql` |
| Tests | NUnit + Testcontainers (PostGIS) — service tests per endpoint |

---

## Solution layout

```
N1che.slnx
├── N1che.Api                    # Minimal-API host: Program.cs, endpoints, DI, exception handling, health
├── N1che.Domain                 # Interfaces, domain services, exceptions — no infrastructure
├── N1che.Contracts              # Request/response DTOs (wire shapes)
├── N1che.Persistence.Postgres   # Npgsql/Dapper readers, writers, transaction scope, goose migrations
└── N1che.ServiceTests           # NUnit + Testcontainers service tests
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

## Prerequisites

- **.NET 10 SDK** — if installed under `~/.dotnet` and not on `PATH`, export it first:
  ```bash
  export DOTNET_ROOT="$HOME/.dotnet"
  export PATH="$HOME/.dotnet:$PATH"
  ```
- **Docker** — for the local Postgres/PostGIS instance and Testcontainers.
- **goose** — `brew install goose` (used to apply migrations).

---

## Running locally

Start the database (PostGIS), then the API:

```bash
docker compose up -d          # postgis/postgis:16-3.4 on localhost:5432 (db/user/pass all "n1che")
dotnet run --project N1che.Api --launch-profile http
```

> `docker-compose.yml` is gitignored — it's a local-dev convenience, not shipped config.

The API listens on **http://localhost:5180**:

| Route | Purpose |
|---|---|
| `/scalar/v1` | Scalar API reference UI (dev only) |
| `/openapi/v1.json` | OpenAPI document (dev only) |
| `/health/live` | Liveness — process is up (no dependency checks) |
| `/health/ready` | Readiness — Postgres reachable |

Connection string lives in `N1che.Api/appsettings.json` under `Postgres:ConnectionString`.

---

## Migrations

DDL lives as goose migrations in `N1che.Persistence.Postgres/Migrations/`, applied out-of-process against
the running database:

```bash
goose -dir N1che.Persistence.Postgres/Migrations \
  postgres "host=localhost port=5432 user=n1che password=n1che dbname=n1che sslmode=disable" up
```

The API service owns the schema; the ingestion worker (separate repo) only reads/writes agreed `shops`
columns and never migrates.

---

## Testing

Service tests run against a real PostGIS container via Testcontainers — no shared/local DB required:

```bash
dotnet test
```

Every endpoint must have a passing service test before it is considered complete.
