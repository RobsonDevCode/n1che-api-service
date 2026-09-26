using Dapper;
using N1che.Domain.Extensions;
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

    private static int CurrentDayOfWeek => DateTime.UtcNow.ToDayOfWeekIndex();

    public async Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT shops.id,
                   shops.google_place_id,
                   shops.name,
                   shops.niches,
                   shops.address,
                   ST_Y(shops.location::geometry) AS latitude,
                   ST_X(shops.location::geometry) AS longitude,
                   shops.vote_count,
                   shops.place_status,
                   shop_hours.open_time,
                   shop_hours.close_time,
                   shops.created_at,
                   shops.added_by_user_id,
                   shops.added_by_username
            FROM shops
            LEFT JOIN shop_hours ON shop_hours.shop_id = shops.id
                                AND shop_hours.day_of_week = @DayOfWeek
            WHERE ST_DWithin(shops.location, ST_MakePoint(@Longitude, @Latitude)::geography, @RadiusMeters)
            AND (cardinality(@Niches) = 0 OR shops.niches && @Niches)
            ORDER BY shops.location <-> ST_MakePoint(@Longitude, @Latitude)::geography
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
            DayOfWeek = CurrentDayOfWeek,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopCompositeEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }

    public async Task<PaginationModel<ShopModel>> GetPage(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken)
    {
        const string pageSql =
            """
            SELECT shops.id,
                   shops.google_place_id,
                   shops.name,
                   shops.niches,
                   shops.address,
                   ST_Y(shops.location::geometry) AS latitude,
                   ST_X(shops.location::geometry) AS longitude,
                   shops.vote_count,
                   shops.place_status,
                   shop_hours.open_time,
                   shop_hours.close_time,
                   shops.created_at,
                   shops.added_by_user_id,
                   shops.added_by_username
            FROM shops
            LEFT JOIN shop_hours ON shop_hours.shop_id = shops.id
                                AND shop_hours.day_of_week = @DayOfWeek
            WHERE (cardinality(@Niches) = 0 OR shops.niches && @Niches)
            ORDER BY shops.vote_count DESC, shops.created_at DESC
            LIMIT @Limit OFFSET @Offset
            """;

   
        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            Niches = filterModel.Niche ?? [],
            Limit = pagination.PageSize,
            Offset = (pagination.Page - 1) * pagination.PageSize,
            DayOfWeek = CurrentDayOfWeek,
        };

        var pageCommand = new CommandDefinition(pageSql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopCompositeEntity>(pageCommand);

        var totalCount = await GetCount(filterModel, cancellationToken);

        return new PaginationModel<ShopModel>
        {
            Data = entities.Select(entity => entity.ToDomainModel()).ToArray(),
            TotalCount = totalCount,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
        };
    }

    public async Task<ShopModel?> GetById(Guid id, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT shops.id,
                   shops.google_place_id,
                   shops.name,
                   shops.niches,
                   shops.address,
                   ST_Y(shops.location::geometry) AS latitude,
                   ST_X(shops.location::geometry) AS longitude,
                   shops.vote_count,
                   shops.place_status,
                   shop_hours.open_time,
                   shop_hours.close_time,
                   shops.created_at,
                   shops.added_by_user_id,
                   shops.added_by_username
            FROM shops
            LEFT JOIN shop_hours ON shop_hours.shop_id = shops.id
                                AND shop_hours.day_of_week = @DayOfWeek
            WHERE shops.id = @Id
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { Id = id, DayOfWeek = CurrentDayOfWeek }, cancellationToken: cancellationToken);
        var entity = await connection.QuerySingleOrDefaultAsync<ShopCompositeEntity>(command);

        return entity?.ToDomainModel();
    }

    public async Task<IReadOnlyCollection<ShopModel>> GetByIds(IReadOnlyCollection<Guid> ids, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT shops.id,
                   shops.google_place_id,
                   shops.name,
                   shops.niches,
                   shops.address,
                   ST_Y(shops.location::geometry) AS latitude,
                   ST_X(shops.location::geometry) AS longitude,
                   shops.vote_count,
                   shops.place_status,
                   shop_hours.open_time,
                   shop_hours.close_time,
                   shops.created_at,
                   shops.added_by_user_id,
                   shops.added_by_username
            FROM shops
            LEFT JOIN shop_hours ON shop_hours.shop_id = shops.id
                                AND shop_hours.day_of_week = @DayOfWeek
            WHERE shops.id = ANY(@Ids)
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new { Ids = ids.ToArray(), DayOfWeek = CurrentDayOfWeek };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopCompositeEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }

    public async Task<bool> Exists(Guid id, CancellationToken cancellationToken)
    {
        const string sql = "SELECT EXISTS (SELECT 1 FROM shops WHERE id = @Id)";

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<bool>(command);
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
