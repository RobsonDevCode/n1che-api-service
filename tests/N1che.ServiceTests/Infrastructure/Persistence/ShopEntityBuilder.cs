using AutoFixture;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class ShopEntityBuilder
{
    // Each field falls back to a fixture-generated value when the caller does not supply one,
    // so a scenario only has to set the fields it actually asserts on.
    public static ShopEntity Build(
        IFixture fixture,
        double? latitude = null,
        double? longitude = null,
        int? voteCount = null,
        DateTime? createdAt = null,
        params string[] niches)
    {
        var builder = fixture.Build<ShopEntity>()
            .With(shop => shop.CreatedAt, createdAt ?? DateTime.UtcNow);

        if (niches.Length > 0)
        {
            builder = builder.With(shop => shop.Niches, niches);
        }

        if (latitude.HasValue)
        {
            builder = builder.With(shop => shop.Latitude, latitude.Value);
        }

        if (longitude.HasValue)
        {
            builder = builder.With(shop => shop.Longitude, longitude.Value);
        }

        if (voteCount.HasValue)
        {
            builder = builder.With(shop => shop.VoteCount, voteCount.Value);
        }

        return builder.Create();
    }

    public static List<ShopEntity> BuildMany(
        IFixture fixture,
        int count,
        double? latitude = null,
        double? longitude = null,
        int? voteCount = null,
        DateTime? createdAt = null,
        params string[] niches) =>
        Enumerable.Range(0, count)
            .Select(_ => Build(fixture, latitude, longitude, voteCount, createdAt, niches))
            .ToList();
}
