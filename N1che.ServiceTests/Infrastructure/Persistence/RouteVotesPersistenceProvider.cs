using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class RouteVotesPersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    internal static async Task Insert(Guid routeId, string userId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        await connection.ExecuteAsync(
            """
            INSERT INTO route_votes (route_id, user_id)
            VALUES (@RouteId, @UserId)
            """,
            new { RouteId = routeId, UserId = userId });
    }

    internal static async Task<IEnumerable<RouteVoteEntity>> GetByRouteId(Guid routeId)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return await connection.QueryAsync<RouteVoteEntity>(
            """
            SELECT route_id, user_id, created_at
            FROM route_votes
            WHERE route_id = @routeId
            ORDER BY created_at
            """,
            new { routeId });
    }
}
