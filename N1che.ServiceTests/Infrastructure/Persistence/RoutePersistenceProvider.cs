using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class RoutePersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    // Dapper leaves an unselected member at its default, so every route read shares one projection
    // rather than risking a new column reaching one query and not the other.
    private const string RouteProjection =
        """
        SELECT id,
               name,
               tag,
               niche,
               created_by_user_id,
               created_by_username,
               ST_Y(anchor::geometry) AS anchor_latitude,
               ST_X(anchor::geometry) AS anchor_longitude,
               ST_AsGeoJSON(polyline) AS polyline_geo_json,
               distance_meters,
               total_minutes,
               vote_count,
               created_at,
               updated_at
        FROM routes
        """;

    // Stops must already exist as shops — route_stops.shop_id references them.
    internal static async Task Insert(RouteEntity route, IReadOnlyList<ShopEntity> stops)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            INSERT INTO routes (id, name, tag, niche, created_by_user_id, created_by_username, anchor, polyline, distance_meters, total_minutes, vote_count, created_at, updated_at)
            VALUES (@Id, @Name, @Tag, @Niche, @CreatedByUserId, @CreatedByUsername,
                    ST_MakePoint(@AnchorLongitude, @AnchorLatitude)::geography,
                    ST_GeomFromGeoJSON(@PolylineGeoJson)::geography,
                    @DistanceMeters, @TotalMinutes, @VoteCount, @CreatedAt, @UpdatedAt)
            """,
            route);

        for (var position = 0; position < stops.Count; position++)
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO route_stops (route_id, shop_id, position)
                VALUES (@RouteId, @ShopId, @Position)
                """,
                new { RouteId = route.Id, ShopId = stops[position].Id, Position = position });
        }
    }

    internal static async Task<RouteEntity> GetByName(string name)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.QuerySingleAsync<RouteEntity>(
            $"{RouteProjection} WHERE name = @name",
            new { name });
    }

    internal static async Task<int> CountByName(string name)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM routes WHERE name = @name",
            new { name });
    }

    internal static async Task<IReadOnlyCollection<RouteStopEntity>> GetStops(Guid routeId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var stops = await connection.QueryAsync<RouteStopEntity>(
            """
            SELECT route_id, shop_id, position
            FROM route_stops
            WHERE route_id = @routeId
            ORDER BY position
            """,
            new { routeId });

        return stops.ToArray();
    }
}
