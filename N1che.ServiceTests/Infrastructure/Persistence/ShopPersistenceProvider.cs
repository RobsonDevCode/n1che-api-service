using Dapper;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using Npgsql;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ShopPersistenceProvider
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("Postgres__ConnectionString")
        ?? throw new InvalidOperationException("Postgres__ConnectionString is not set");

    internal static async Task Insert(IEnumerable<ShopEntity> shops)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        foreach (var shop in shops)
        {
            await connection.ExecuteAsync(
                """
                INSERT INTO shops (id, google_place_id, name, niches, address, location, vote_count, place_status, created_at, added_by_user_id, added_by_username)
                VALUES (@Id, @GooglePlaceId, @Name, @Niches, @Address, ST_MakePoint(@Longitude, @Latitude)::geography, @VoteCount, @PlaceStatus, @CreatedAt, @AddedByUserId, @AddedByUsername)
                """,
                shop);
        }
    }
}
