using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class BookmarksPersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    internal static async Task Insert(Guid shopId, string userId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            INSERT INTO bookmarks (shop_id, user_id)
            VALUES (@ShopId, @UserId)
            """,
            new { ShopId = shopId, UserId = userId });
    }

    internal static async Task<IReadOnlyCollection<BookmarkEntity>> GetByShopId(Guid shopId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var bookmarks = await connection.QueryAsync<BookmarkEntity>(
            """
            SELECT shop_id, user_id, created_at
            FROM bookmarks
            WHERE shop_id = @shopId
            ORDER BY created_at
            """,
            new { shopId });

        return bookmarks.ToArray();
    }
}
