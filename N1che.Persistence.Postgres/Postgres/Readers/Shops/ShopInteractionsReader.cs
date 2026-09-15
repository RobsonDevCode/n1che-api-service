using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.Persistence.Postgres.Postgres.Extensions.Shops;

namespace N1che.Persistence.Postgres.Postgres.Readers.Shops;

public sealed class ShopInteractionsReader : IShopInteractionsReader
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopInteractionsReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ShopInteractionsModel?> GetByShopId(Guid shopId, string userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT shops.vote_count,
                   EXISTS (SELECT 1
                           FROM shop_votes
                           WHERE shop_votes.shop_id = shops.id
                           AND shop_votes.user_id = @UserId) AS voted,
                   EXISTS (SELECT 1
                           FROM bookmarks
                           WHERE bookmarks.shop_id = shops.id
                           AND bookmarks.user_id = @UserId) AS saved
            FROM shops
            WHERE shops.id = @ShopId
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { ShopId = shopId, UserId = userId }, cancellationToken: cancellationToken);
        var entity = await connection.QuerySingleOrDefaultAsync<ShopInteractionsCompositeEntity>(command);

        return entity?.ToDomainModel();
    }
}
