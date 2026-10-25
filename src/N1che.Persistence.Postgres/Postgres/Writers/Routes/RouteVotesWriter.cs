using Dapper;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Writers.Routes;
using N1che.Persistence.Postgres.Postgres.Connections;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Writers.Routes;

public sealed class RouteVotesWriter : IRouteVotesWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public RouteVotesWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Create(Guid routeId, string userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO route_votes (route_id, user_id)
            VALUES (@RouteId, @UserId)
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { RouteId = routeId, UserId = userId }, cancellationToken: cancellationToken);

        try
        {
            await connection.ExecuteAsync(command);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new DuplicateRequestException(EntityTypes.RouteVote, routeId);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw new NotFoundException(EntityTypes.Route, routeId);
        }
    }

    public async Task Delete(Guid routeId, string userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            DELETE FROM route_votes
            WHERE route_id = @RouteId
            AND user_id = @UserId
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { RouteId = routeId, UserId = userId }, cancellationToken: cancellationToken);

        if (await connection.ExecuteAsync(command) == 0)
        {
            throw new NotFoundException(EntityTypes.RouteVote, routeId);
        }
    }
}
