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

    // Stops must already exist as shops — route_stops.shop_id references them.
    internal static async Task Insert(RouteEntity route, IReadOnlyList<ShopEntity> stops)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            INSERT INTO routes (id, name, tag, mode, niche, created_by_user_id, created_by_username, anchor, polyline, distance_meters, total_minutes, vote_count, created_at, updated_at)
            VALUES (@Id, @Name, @Tag, @Mode, @Niche, @CreatedByUserId, @CreatedByUsername,
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
}
