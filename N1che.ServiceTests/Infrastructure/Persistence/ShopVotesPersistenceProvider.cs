using Dapper;
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
}
