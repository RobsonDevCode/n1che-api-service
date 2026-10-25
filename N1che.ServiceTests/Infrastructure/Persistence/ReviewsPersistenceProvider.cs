using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ReviewsPersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    internal static async Task Insert(IEnumerable<ReviewEntity> reviews)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        foreach (var review in reviews)
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO reviews (id, shop_id, niche, user_id, username, body, update_count, created_at, updated_at)
                VALUES (@Id, @ShopId, @Niche, @UserId, @Username, @Body, @UpdateCount, @CreatedAt, @UpdatedAt)
                """,
                review);
        }
    }

    internal static async Task<IEnumerable<ReviewEntity>> GetByShopId(Guid shopId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.QueryAsync<ReviewEntity>(
            """
            SELECT id, shop_id, niche, user_id, username, body, update_count, created_at, updated_at
            FROM reviews
            WHERE shop_id = @ShopId
            """,
            new { ShopId = shopId });
    }
}
