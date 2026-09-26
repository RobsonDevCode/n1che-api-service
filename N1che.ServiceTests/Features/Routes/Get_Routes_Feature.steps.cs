using System.Globalization;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using N1che.Contracts.Filters.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Routes_Feature : FeatureFixture
{
    private const double SmallRadiusMeters = 500;

    // Mirror the API defaults applied when the caller supplies neither.
    private const double DefaultRadiusMeters = 5000;
    private const int DefaultLimit = 20;

    // Offsets in degrees of latitude from this scenario's origin — roughly 111m, 222m, 1.1km and 22km.
    private const double NearOffset = 0.001;
    private const double NearerOffset = 0.002;
    private const double MidOffset = 0.01;
    private const double FarOffset = 0.2;

    // Keeps a route's two stops distinct so its polyline is a real line rather than a point.
    private const double StopSpacing = 0.00002;

    private readonly IFixture _fixture;

    // A fresh origin per scenario keeps each scenario's rows out of every other scenario's radius.
    private readonly double _originLatitude;
    private readonly double _originLongitude;

    // Seed timestamps are spaced off this so the vote-count tie-break resolves deterministically.
    private readonly DateTime _seedTime;

    // Empty until a scenario seeds, so the unseeded scenario expects an empty response.
    private List<(RouteEntity Route, ShopEntity[] Stops)> _routes = [];
    private RoutesFilter _filter = null!;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    private readonly string EndpointLog;
    private const string SuccessLog = "Routes retrieved";

    public Get_Routes_Feature()
    {
        _fixture = new Fixture();
        _originLatitude = Random.Shared.Next(-80, 80) + Random.Shared.NextDouble();
        _originLongitude = Random.Shared.Next(-170, 170) + Random.Shared.NextDouble();
        _seedTime = DateTime.UtcNow;

        EndpointLog = string.Format(CultureInfo.InvariantCulture,
            "Getting top rated routes at ({0}, {1}) for niche {2}", _originLatitude, _originLongitude, NicheConstants.Goth);
    }

    private async Task Routes_Exist()
    {
        _routes =
        [
            await SeedRoute(NearOffset, NicheConstants.Goth, voteCount: 40, _seedTime),
            await SeedRoute(NearerOffset, NicheConstants.Goth, voteCount: 90, _seedTime.AddSeconds(-1)),
            // Three stops running north to south, so position order is the reverse of latitude order and
            // a reader ordering by anything other than position hands them back the wrong way round.
            await SeedRoute(NicheConstants.Goth, voteCount: 60, _seedTime.AddSeconds(-2),
                [MidOffset + StopSpacing, MidOffset, MidOffset - StopSpacing]),
            await SeedRoute(FarOffset, NicheConstants.Goth, voteCount: 100, _seedTime.AddSeconds(-3)),
            await SeedRoute(NearOffset, NicheConstants.Vintage, voteCount: 70, _seedTime.AddSeconds(-4))
        ];
    }

    private async Task GetRoutes_Is_Called(RoutesFilter filter)
    {
        _filter = filter;
        _scopeValues = new Dictionary<string, object>
        {
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng,
            ["Niche"] = filter.Niche
        };

        _response = await Client.GetRoutes(filter);
    }

    private async Task The_Routes_Matching_The_Filter_Are_Returned()
    {
        var routes = await _response.Content.ReadFromJsonAsync<IReadOnlyCollection<RouteResponse>>();
        routes.Should().NotBeNull();

        var radius = _filter.Radius ?? DefaultRadiusMeters;

        // A route is anchored at the centroid of its stops, so averaging them reproduces its anchor.
        var expected = _routes
            .Where(seed => seed.Route.Niche == _filter.Niche)
            .Where(seed => DistanceMeters(_filter.Lat, _filter.Lng,
                seed.Stops.Average(stop => stop.Latitude), seed.Stops.Average(stop => stop.Longitude)) <= radius)
            .OrderByDescending(seed => seed.Route.VoteCount)
            .ThenByDescending(seed => seed.Route.CreatedAt)
            .Take(_filter.Limit ?? DefaultLimit)
            .Select(Expected)
            .ToArray();

        routes.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Contains_A_Validation_Error_For(string field)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(field);
    }

    private Task<(RouteEntity Route, ShopEntity[] Stops)> SeedRoute(
        double latitudeOffset, string niche, int voteCount, DateTime createdAt) =>
        SeedRoute(niche, voteCount, createdAt, [latitudeOffset, latitudeOffset + StopSpacing]);

    private async Task<(RouteEntity Route, ShopEntity[] Stops)> SeedRoute(
        string niche, int voteCount, DateTime createdAt, IReadOnlyList<double> stopLatitudeOffsets)
    {
        var stops = stopLatitudeOffsets.Select(offset => ShopAt(offset, niche)).ToArray();

        await ShopPersistenceProvider.Insert(stops);

        var route = RouteEntityBuilder.Build(_fixture, stops, niche, voteCount, createdAt);
        await RoutePersistenceProvider.Insert(route, stops);

        return (route, stops);
    }

    private ShopEntity ShopAt(double latitudeOffset, string niche) =>
        ShopEntityBuilder.Build(_fixture,
            latitude: _originLatitude + latitudeOffset,
            longitude: _originLongitude,
            niches: niche);

    private static RouteResponse Expected((RouteEntity Route, ShopEntity[] Stops) seed) => new()
    {
        Id = seed.Route.Id,
        Name = seed.Route.Name,
        Tag = seed.Route.Tag,
        Mode = seed.Route.Mode,
        Niche = seed.Route.Niche,
        CreatedBy = seed.Route.CreatedByUsername,
        UserId = seed.Route.CreatedByUserId,
        Stops = seed.Stops.Select(stop => new RouteStopResponse
        {
            Id = stop.Id,
            Name = stop.Name,
            Address = stop.Address,
            Latitude = stop.Latitude,
            Longitude = stop.Longitude,
            PlaceStatus = stop.PlaceStatus,
        }).ToArray(),
        Polyline = seed.Stops.Select(stop => new CoordinateResponse
        {
            Latitude = stop.Latitude,
            Longitude = stop.Longitude,
        }).ToArray(),
        DistanceMeters = seed.Route.DistanceMeters,
        TotalMinutes = seed.Route.TotalMinutes,
        TotalUpvotes = seed.Route.VoteCount,
        CreatedAt = seed.Route.CreatedAt,
    };

    private static double DistanceMeters(double lat1, double lng1, double lat2, double lng2)
    {
        const double earthRadiusMeters = 6371000;
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLng = (lng2 - lng1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return 2 * earthRadiusMeters * Math.Asin(Math.Sqrt(a));
    }
}
