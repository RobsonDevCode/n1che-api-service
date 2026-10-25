using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ShopVotesPersistenceProvider
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
            INSERT INTO shop_votes (shop_id, user_id)
            VALUES (@ShopId, @UserId)
            """,
            new { ShopId = shopId, UserId = userId });
    }

    internal static async Task<IReadOnlyCollection<ShopVoteEntity>> GetByShopId(Guid shopId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        var votes = await connection.QueryAsync<ShopVoteEntity>(
            """
            SELECT shop_id, user_id, created_at
            FROM shop_votes
            WHERE shop_id = @shopId
            ORDER BY created_at
            """,
            new { shopId });

        return votes.ToArray();
    }
}
