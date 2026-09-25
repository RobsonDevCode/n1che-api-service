using Dapper;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Writers.Shops;

public sealed class ShopVotesWriter : IShopVotesWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopVotesWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task Create(Guid shopId, string userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO shop_votes (shop_id, user_id)
            VALUES (@ShopId, @UserId)
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { ShopId = shopId, UserId = userId }, cancellationToken: cancellationToken);

        try
        {
            await connection.ExecuteAsync(command);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new DuplicateRequestException(EntityTypes.Vote, shopId);
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            throw new NotFoundException(EntityTypes.Shop, shopId);
        }
    }

    public async Task<bool> Delete(Guid shopId, string userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            DELETE FROM shop_votes
            WHERE shop_id = @ShopId
            AND user_id = @UserId
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { ShopId = shopId, UserId = userId }, cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command) > 0;
    }
}
