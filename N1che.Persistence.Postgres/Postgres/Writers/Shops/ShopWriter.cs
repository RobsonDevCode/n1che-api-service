using Dapper;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Writers.Shops;
using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.Persistence.Postgres.Postgres.Extensions.Shops;
using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Writers.Shops;

public sealed class ShopWriter : IShopsWriter
{
    private readonly PostgresConnectionFactory _connectionFactory;

    public ShopWriter(PostgresConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ShopModel> Create(NewShopModel newShop, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO shops (google_place_id, name, niches, address, location, added_by_user_id, added_by_username)
            VALUES (@GooglePlaceId,
                    @Name,
                    @Niches,
                    @Address,
                    ST_MakePoint(@Longitude, @Latitude)::geography,
                    @AddedByUserId,
                    @AddedByUsername)
            RETURNING id,
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
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var parameters = new
        {
            newShop.GooglePlaceId,
            newShop.Name,
            Niches = newShop.Niches.ToArray(),
            newShop.Address,
            newShop.Latitude,
            newShop.Longitude,
            newShop.AddedByUserId,
            newShop.AddedByUsername,
        };

        var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);

        try
        {
            var entity = await connection.QuerySingleAsync<ShopEntity>(command);
            return entity.ToDomainModel();
        }
        catch (PostgresException exception) when (exception.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            throw new DuplicateRequestException(EntityTypes.Shop, newShop.GooglePlaceId);
        }
    }

    public async Task IncrementVoteCount(Guid shopId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            UPDATE shops
            SET vote_count = vote_count + 1,
                updated_at = now()
            WHERE id = @ShopId
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { ShopId = shopId }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }

    public async Task DecrementVoteCount(Guid shopId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            UPDATE shops
            SET vote_count = GREATEST(vote_count - 1, 0),
                updated_at = now()
            WHERE id = @ShopId
            """;

        await using var connection = await _connectionFactory.ConnectAsync(cancellationToken);

        var command = new CommandDefinition(sql, new { ShopId = shopId }, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
