using System.Globalization;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using N1che.Contracts.Filters.Shops;
using N1che.Contracts.Response.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Nearby_Shops_Feature : FeatureFixture
{
    private const double SmallRadiusMeters = 500;
    private const double DefaultRadiusMeters = 5000; // mirrors the API default when no radius is supplied

    private const string Goth = "goth";
    private const string Skater = "skater";
    private const string Vintage = "vintage";
    private const string Streetwear = "streetwear";
    private const string Y2k = "y2k";

    private readonly IFixture _fixture;

    // A fresh origin per scenario keeps each scenario's rows out of every other scenario's radius.
    private readonly double _originLatitude;
    private readonly double _originLongitude;

    private List<ShopEntity> _shops = null!;
    private NearbyShopsFilter _filter = null!;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateClient();

    private readonly string EndpointLog;
    private const string SuccessLog = "Nearby shops retrieved";

    public Get_Nearby_Shops_Feature()
    {
        _fixture = new Fixture();
        _originLatitude = Random.Shared.Next(-80, 80) + Random.Shared.NextDouble();
        _originLongitude = Random.Shared.Next(-170, 170) + Random.Shared.NextDouble();

        EndpointLog = string.Format(CultureInfo.InvariantCulture,
            "Getting nearby shops at ({0}, {1}) for niche {2}", _originLatitude, _originLongitude, Goth);
    }

    private async Task Shops_Exist()
    {
        _shops =
        [
            ShopAt(latitudeOffset: 0.001, Goth),
            ShopAt(latitudeOffset: 0.01, Goth),
            ShopAt(latitudeOffset: 0.2, Goth),
            ShopAt(latitudeOffset: 0.0005, Skater),
            ShopAt(latitudeOffset: 0.001, Vintage),
            ShopAt(latitudeOffset: 0.01, Vintage),
            ShopAt(latitudeOffset: 0.001, Streetwear, Y2k),
            ShopAt(latitudeOffset: 0.002, Streetwear)
        ];

        await ShopPersistenceProvider.Insert(_shops);
    }

    private async Task GetNearbyShops_Is_Called(NearbyShopsFilter filter)
    {
        _filter = filter;
        _scopeValues = new Dictionary<string, object>
        {
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng,
            ["Niche"] = filter.Niche ?? "all"
        };

        _response = await Client.GetNearbyShops(filter);
    }

    private async Task The_Shops_Matching_The_Filter_Are_Returned()
    {
        var shops = await _response.Content.ReadFromJsonAsync<IReadOnlyCollection<ShopResponse>>();
        shops.Should().NotBeNull();

        var radius = _filter.Radius ?? DefaultRadiusMeters;
        var expected = _shops
            .Where(shop => _filter.Niche is null || shop.Niches.Contains(_filter.Niche))
            .Select(shop => (shop, distance: DistanceMeters(_filter.Lat, _filter.Lng, shop.Latitude, shop.Longitude)))
            .Where(x => x.distance <= radius)
            .OrderBy(x => x.distance)
            .Select(x => Expected(x.shop))
            .ToArray();

        shops.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Contains_A_Validation_Error_For(string field)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ValidationProblem>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(field);
    }

    private sealed record ValidationProblem(Dictionary<string, string[]> Errors);

    private ShopEntity ShopAt(double latitudeOffset, params string[] niches) =>
        _fixture.Build<ShopEntity>()
            .With(shop => shop.Niches, niches)
            .With(shop => shop.Latitude, _originLatitude + latitudeOffset)
            .With(shop => shop.Longitude, _originLongitude)
            .With(shop => shop.CreatedAt, DateTime.UtcNow)
            .Create();

    private static ShopResponse Expected(ShopEntity shop) => new()
    {
        Id = shop.Id.ToString(),
        GooglePlaceId = shop.GooglePlaceId,
        Name = shop.Name,
        Niches = shop.Niches,
        Address = shop.Address,
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
        VoteCount = shop.VoteCount,
        PlaceStatus = shop.PlaceStatus,
        CreatedAt = shop.CreatedAt,
        AddedByUserId = shop.AddedByUserId,
        AddedByUsername = shop.AddedByUsername,
        PhotoUrl = null
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
