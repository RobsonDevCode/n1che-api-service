using Dapper;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.Persistence.Postgres.Postgres.Extensions.Shops;

namespace N1che.Persistence.Postgres.Postgres.Readers.Shops;

public sealed class ShopReader : IShopsReader
{
    // <-> is the PostGIS KNN distance operator; ORDER BY location <-> point sorts nearest-first via the GiST index.
    private const string SelectNearbySql =
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
          AND (@Niches IS NULL OR niches && @Niches)
        ORDER BY location <-> ST_MakePoint(@Longitude, @Latitude)::geography
        LIMIT @Limit
        """;

    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopReader(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            filter.Latitude,
            filter.Longitude,
            filter.RadiusMeters,
            Niches = filter.Niche is null ? null : new[] { filter.Niche },
            filter.Limit,
        };

        var command = new CommandDefinition(SelectNearbySql, parameters, cancellationToken: cancellationToken);
        var entities = await connection.QueryAsync<ShopEntity>(command);

        return entities.Select(entity => entity.ToDomainModel()).ToArray();
    }
}
