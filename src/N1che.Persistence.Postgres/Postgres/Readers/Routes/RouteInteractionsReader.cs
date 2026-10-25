using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Extensions.Routes;

namespace N1che.Persistence.Postgres.Postgres.Readers.Routes;

public sealed class RouteInteractionsReader : IRouteInteractionsReader
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public RouteInteractionsReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<RouteInteractionsModel?> GetById(Guid routeId, string? userId, CancellationToken cancellationToken)
    {
        var votedColumn = userId is null
            ? "FALSE"
            : """
              EXISTS (SELECT 1
                      FROM route_votes
                      WHERE route_votes.route_id = routes.id
                      AND route_votes.user_id = @UserId)
              """;

        var sql =
            $"""
             SELECT routes.vote_count,
                    {votedColumn} AS voted
             FROM routes
             WHERE routes.id = @RouteId
             """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { RouteId = routeId, UserId = userId }, cancellationToken: cancellationToken);
        var entity = await connection.QuerySingleOrDefaultAsync<RouteInteractionsCompositeEntity>(command);

        return entity?.ToDomainModel();
    }
}
