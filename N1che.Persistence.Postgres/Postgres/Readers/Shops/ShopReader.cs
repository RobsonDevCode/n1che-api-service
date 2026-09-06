using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.Persistence.Postgres.Postgres.Extensions.Shops;

namespace N1che.Persistence.Postgres.Postgres.Readers.Shops;

public sealed class ShopReader : IShopsReader
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT id,
                   google_place_id,
                   name,
                   niches,
                   address,
                   ST_Y(location::geometry) AS latitude,
                   ST_X(location::geometry) AS longitude,
                   vote_count,
                   place_status,
                   created_at,
                   added_by_user_id,
                   added_by_username
            FROM shops
            WHERE ST_DWithin(location, ST_MakePoint(@Longitude, @Latitude)::geography, @RadiusMeters)
            AND (cardinality(@Niches) = 0 OR niches && @Niches)
            ORDER BY location <-> ST_MakePoint(@Longitude, @Latitude)::geography
            LIMIT @Limit
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            filter.Latitude,
            filter.Longitude,
            filter.RadiusMeters,
            Niches = filter.Niche is null ? [] : new[] { filter.Niche },
            filter.Limit,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }

    public async Task<PaginationModel<ShopModel>> GetPage(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken)
    {
        const string pageSql =
            """
            SELECT id,
                   google_place_id,
                   name,
                   niches,
                   address,
                   ST_Y(location::geometry) AS latitude,
                   ST_X(location::geometry) AS longitude,
                   vote_count,
                   place_status,
                   created_at,
                   added_by_user_id,
                   added_by_username
            FROM shops
            WHERE (cardinality(@Niches) = 0 OR niches && @Niches)
            ORDER BY vote_count DESC, created_at DESC
            LIMIT @Limit OFFSET @Offset
            """;

   
        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            Niches = filterModel.Niche ?? [],
            Limit = pagination.PageSize,
            Offset = (pagination.Page - 1) * pagination.PageSize,
        };

        var pageCommand = new CommandDefinition(pageSql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopEntity>(pageCommand);

        var totalCount = await GetCount(filterModel, cancellationToken);

        return new PaginationModel<ShopModel>
        {
            Data = entities.Select(entity => entity.ToDomainModel()).ToArray(),
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
        };
    }

    public async Task<long> GetCount(ShopsFilterModel filterModel, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT COUNT(*)
            FROM shops
            WHERE (cardinality(@Niches) = 0 OR niches && @Niches)
            """;

        
        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);
        var command = new CommandDefinition(sql, new { Niches = filterModel.Niche ?? [] }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<long>(command);
    }
}
