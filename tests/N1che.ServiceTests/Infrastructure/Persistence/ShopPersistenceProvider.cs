using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ShopPersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    // Dapper leaves an unselected member at its default, so every shop read shares one projection
    // rather than risking a new column reaching one query and not the other.
    private const string ShopProjection =
        """
        SELECT id,
               google_place_id,
               name,
               niches,
               address,
               ST_Y(location::geometry) AS latitude,
               ST_X(location::geometry) AS longitude,
               vote_count,
               place_status,
               created_at,
               added_by_user_id,
               added_by_username
        FROM shops
        """;

    internal static async Task Insert(IEnumerable<ShopEntity> shops)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        foreach (var shop in shops)
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO shops (id, google_place_id, name, niches, address, location, vote_count, place_status, created_at, added_by_user_id, added_by_username)
                VALUES (@Id, @GooglePlaceId, @Name, @Niches, @Address, ST_MakePoint(@Longitude, @Latitude)::geography, @VoteCount, @PlaceStatus, @CreatedAt, @AddedByUserId, @AddedByUsername)
                """,
                shop);
        }
    }

    internal static async Task Update(ShopEntity shop)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            UPDATE shops
            SET google_place_id = @GooglePlaceId,
                name = @Name,
                niches = @Niches,
                address = @Address,
                location = ST_MakePoint(@Longitude, @Latitude)::geography,
                vote_count = @VoteCount,
                place_status = @PlaceStatus,
                created_at = @CreatedAt,
                added_by_user_id = @AddedByUserId,
                added_by_username = @AddedByUsername
            WHERE id = @Id
            """,
            shop);
    }

    internal static async Task InsertHours(IEnumerable<ShopHoursEntity> hours)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            INSERT INTO shop_hours (id, shop_id, day_of_week, open_time, close_time)
            VALUES (@Id, @ShopId, @DayOfWeek, @OpenTime, @CloseTime)
            """,
            hours);
    }

    internal static async Task<ShopEntity> GetByGooglePlaceId(string googlePlaceId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.QuerySingleAsync<ShopEntity>(
            $"{ShopProjection} WHERE google_place_id = @googlePlaceId",
            new { googlePlaceId });
    }

    internal static async Task<ShopEntity> GetById(Guid id)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.QuerySingleAsync<ShopEntity>(
            $"{ShopProjection} WHERE id = @id",
            new { id });
    }

    internal static async Task<int> CountByGooglePlaceId(string googlePlaceId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(*) FROM shops WHERE google_place_id = @googlePlaceId",
            new { googlePlaceId });
    }

    internal static async Task<IReadOnlyCollection<ShopHoursEntity>> GetHours(Guid shopId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var hours = await connection.QueryAsync<ShopHoursEntity>(
            """
            SELECT id, shop_id, day_of_week, open_time, close_time
            FROM shop_hours
            WHERE shop_id = @shopId
            ORDER BY day_of_week
            """,
            new { shopId });

        return hours.ToArray();
    }
}
