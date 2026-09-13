using System.Text.Json;
using AutoFixture;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.ServiceTests.Infrastructure.Persistence;

internal static class RouteEntityBuilder
{
    private const string DefaultMode = "you";

    // Each field falls back to a fixture-generated value when the caller does not supply one,
    // so a scenario only has to set the fields it actually asserts on.
    public static RouteEntity Build(
        IFixture fixture,
        IReadOnlyList<ShopEntity> stops,
        string niche,
        int? voteCount = null,
        DateTime? createdAt = null,
        string mode = DefaultMode)
    {
        var builder = fixture.Build<RouteEntity>()
            .With(route => route.Niche, niche)
            .With(route => route.Mode, mode)
            .With(route => route.CreatedAt, createdAt ?? DateTime.UtcNow)
            .With(route => route.AnchorLatitude, stops.Average(stop => stop.Latitude))
            .With(route => route.AnchorLongitude, stops.Average(stop => stop.Longitude))
            .With(route => route.PolylineGeoJson, LineStringThrough(stops));

        if (voteCount.HasValue)
        {
            builder = builder.With(route => route.VoteCount, voteCount.Value);
        }

        return builder.Create();
    }

    // The seeded geometry traces the route's own stops, so the polyline read back is predictable.
    private static string LineStringThrough(IReadOnlyList<ShopEntity> stops) => JsonSerializer.Serialize(new
    {
        type = "LineString",
        coordinates = stops.Select(stop => new[] { stop.Longitude, stop.Latitude }).ToArray()
    });
}
