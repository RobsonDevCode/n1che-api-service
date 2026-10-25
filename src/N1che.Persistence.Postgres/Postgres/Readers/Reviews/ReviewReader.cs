using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Reviews;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Reviews;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using N1che.Persistence.Postgres.Postgres.Extensions.Reviews;

namespace N1che.Persistence.Postgres.Postgres.Readers.Reviews;

public sealed class ReviewReader : IReviewsReader
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ReviewReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<PaginationModel<ReviewModel>> GetPageByShopId(Guid shopId, string niche, PaginationDetailsModel pagination, CancellationToken cancellationToken)
    {
        const string pageSql =
            """
            SELECT id,
                   shop_id,
                   niche,
                   user_id,
                   username,
                   body,
                   update_count,
                   created_at,
                   updated_at
            FROM reviews
            WHERE shop_id = @ShopId
            AND niche = @Niche
            ORDER BY created_at DESC, id DESC
            LIMIT @Limit OFFSET @Offset
            """;

        const string countSql =
            """
            SELECT COUNT(*)
            FROM reviews
            WHERE shop_id = @ShopId
            AND niche = @Niche
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            ShopId = shopId,
            Niche = niche,
            Limit = pagination.PageSize,
            Offset = (pagination.Page - 1) * pagination.PageSize,
        };

        var pageCommand = new CommandDefinition(pageSql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ReviewEntity>(pageCommand);

        var countCommand = new CommandDefinition(countSql, new { ShopId = shopId, Niche = niche }, cancellationToken: cancellationToken);
        var totalCount = await connection.ExecuteScalarAsync<long>(countCommand);

        return new PaginationModel<ReviewModel>
        {
            Data = entities.Select(entity => entity.ToDomainModel()).ToArray(),
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
        };
    }
}
