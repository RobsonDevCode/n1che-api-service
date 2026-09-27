using Dapper;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Writers.Routes;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Extensions.Routes;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Writers.Routes;

public sealed class RouteWriter : IRoutesWriter
{
    private const string NicheForeignKey = "routes_niche_fkey";

    private readonly PostgresConnectionFactory _connectionFactory;

    public RouteWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<CreatedRouteModel> Create(
        CreateRouteModel route, CoordinateModel anchor, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO routes (name, tag, niche, created_by_user_id, created_by_username, anchor, polyline, distance_meters, total_minutes)
            VALUES (@Name,
                    @Tag,
                    @Niche,
                    @CreatedByUserId,
                    @CreatedByUsername,
                    ST_MakePoint(@AnchorLongitude, @AnchorLatitude)::geography,
                    ST_GeomFromGeoJSON(@PolylineGeoJson)::geography,
                    @DistanceMeters,
                    @TotalMinutes)
            RETURNING id, created_at
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            route.Name,
            route.Tag,
            route.Niche,
            route.CreatedByUserId,
            route.CreatedByUsername,
            AnchorLatitude = anchor.Latitude,
            AnchorLongitude = anchor.Longitude,
            PolylineGeoJson = GeoJsonLineString.FromCoordinates(route.Polyline),
            route.DistanceMeters,
            route.TotalMinutes,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            var entity = await connection.QuerySingleAsync<CreatedRouteEntity>(command);
            return entity.ToDomainModel();
        }
        catch (PostgresException exception)
            when (exception.SqlState == PostgresErrorCodes.ForeignKeyViolation
                  && exception.ConstraintName == NicheForeignKey)
        {
            throw new InvalidRequestException(EntityTypes.Niche, route.Niche);
        }
    }
}
