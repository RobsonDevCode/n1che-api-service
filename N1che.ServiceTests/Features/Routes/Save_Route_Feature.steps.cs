using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Requests.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Save_Route_Feature : FeatureFixture
{
    private const string EndpointPath = "/routes";
    private const string SuccessLog = "Route saved";
    private const string ValidationTitle = "One or more validation errors occurred.";
    private const string UnrecognisedNiche = "not-a-niche";
    private const string UnrecognisedNicheMessage = $"Niche {UnrecognisedNiche} is not recognised.";

    private const string NameField = "Name";
    private const string TagField = "Tag";
    private const string NicheField = "Niche";
    private const string StopsField = "Stops";
    private const string PolylineField = "Polyline";

    private const string NameValidationMessage = "Name is required.";
    private const string TagValidationMessage = "Tag is required.";
    private const string NicheValidationMessage = "Niche is required.";
    private const string PolylineValidationMessage = "A polyline is required.";
    private const string TooFewStopsValidationMessage = "A route needs at least 2 stops.";
    private const string TooManyStopsValidationMessage = "A route can hold at most 15 stops.";
    private const string RepeatedStopValidationMessage = "A stop can only appear once in a route.";

    private const int TooManyStops = 16;

    // Offsets in degrees of latitude from this scenario's origin — each stop roughly 111m past the last.
    private static readonly double[] StopOffsets = [0.001, 0.002, 0.003];

    private readonly IFixture _fixture;

    // A fresh origin per scenario keeps each scenario's stops out of every other's, and a fresh name
    // keeps its stored row unmatchable by any other scenario's read.
    private readonly double _originLatitude;
    private readonly double _originLongitude;
    private readonly string _routeName;
    private readonly string _routeTag;
    private readonly string _niche;

    // The walk the caller computed before saving. Both are drawn fresh, so no number the stored route
    // carries can be one the service invented rather than one the request sent.
    private readonly double _distanceMeters;
    private readonly int _totalMinutes;

    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _callerUsername = $"user-{Guid.NewGuid()}";

    // The stop nothing seeds, so a request carrying it reaches the store and finds no shop.
    private readonly Guid _unknownStopId = Guid.NewGuid();
    private readonly string _notFoundMessage;

    private readonly CreateRouteRequest _unknownStopRequest;
    private readonly CreateRouteRequest _singleStopRequest;
    private readonly CreateRouteRequest _tooManyStopsRequest;
    private readonly CreateRouteRequest _repeatedStopRequest;
    private readonly CreateRouteRequest _namelessRequest;
    private readonly CreateRouteRequest _taglessRequest;
    private readonly CreateRouteRequest _nicheLessRequest;
    private readonly CreateRouteRequest _polylineLessRequest;

    private ShopEntity[] _stops = [];
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId, _callerUsername));

    public Save_Route_Feature()
    {
        _fixture = new Fixture();
        _originLatitude = Random.Shared.Latitude();
        _originLongitude = Random.Shared.Longitude();
        _routeName = $"route-{Guid.NewGuid()}";
        _routeTag = _fixture.Create<string>();
        _niche = Random.Shared.Niche();
        _distanceMeters = Random.Shared.Meters();
        _totalMinutes = Random.Shared.Minutes();

        _notFoundMessage = $"Shop {_unknownStopId} not found";

        _unknownStopRequest = new CreateRouteRequest
        {
            Name = _routeName,
            Tag = _routeTag,
            Niche = _niche,
            Stops = [_unknownStopId, Guid.NewGuid()],
            Polyline =
            [
                new CoordinateRequest { Latitude = Random.Shared.Latitude(), Longitude = Random.Shared.Longitude() },
                new CoordinateRequest { Latitude = Random.Shared.Latitude(), Longitude = Random.Shared.Longitude() }
            ],
            DistanceMeters = _distanceMeters,
            TotalMinutes = _totalMinutes,
        };

        _singleStopRequest = _unknownStopRequest with { Stops = [_unknownStopId] };
        _tooManyStopsRequest = _unknownStopRequest with
        {
            Stops = Enumerable.Range(0, TooManyStops).Select(_ => Guid.NewGuid()).ToArray()
        };
        _repeatedStopRequest = _unknownStopRequest with { Stops = [_unknownStopId, _unknownStopId] };
        _namelessRequest = _unknownStopRequest with { Name = string.Empty };
        _taglessRequest = _unknownStopRequest with { Tag = string.Empty };
        _nicheLessRequest = _unknownStopRequest with { Niche = string.Empty };
        _polylineLessRequest = _unknownStopRequest with { Polyline = [] };
    }

    private async Task Shops_Exist(int count)
    {
        _stops = StopOffsets.Take(count)
            .Select(offset => ShopEntityBuilder.Build(_fixture,
                latitude: _originLatitude + offset,
                longitude: _originLongitude,
                niches: Random.Shared.Niche()))
            .ToArray();

        await ShopPersistenceProvider.Insert(_stops);
    }

    private async Task CreateRoute_Is_Called(CreateRouteRequest request)
    {
        _endpointLog = $"Saving route through {request.Stops.Count} stops for niche {request.Niche}";
        _scopeValues = new Dictionary<string, object>
        {
            ["Niche"] = request.Niche,
            ["StopCount"] = request.Stops.Count,
            ["UserId"] = _callerId
        };

        _response = await Client.CreateRoute(request);
    }

    private async Task The_Created_Route_Is_Returned()
    {
        var stored = await RoutePersistenceProvider.GetByName(_routeName);
        var created = await _response.Content.ReadFromJsonAsync<CreatedRouteResponse>();

        created.Should().BeEquivalentTo(new CreatedRouteResponse
        {
            Id = stored.Id,
            CreatedAt = stored.CreatedAt,
        }, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());

        _response.Headers.Location.Should().Be($"/routes/{stored.Id}");
    }

    // The route is anchored at the centroid of its stops, so averaging them reproduces the stored anchor.
    private async Task The_Stored_Route_Is_The_Walk_The_Caller_Sent()
    {
        var stored = await RoutePersistenceProvider.GetByName(_routeName);

        stored.Tag.Should().Be(_routeTag);
        stored.Niche.Should().Be(_niche);
        stored.CreatedByUserId.Should().Be(_callerId);
        stored.CreatedByUsername.Should().Be(_callerUsername);
        stored.VoteCount.Should().Be(0);
        stored.DistanceMeters.Should().BeApproximately(_distanceMeters, 1e-6);
        stored.TotalMinutes.Should().Be(_totalMinutes);
        stored.AnchorLatitude.Should().BeApproximately(_stops.Average(stop => stop.Latitude), 1e-6);
        stored.AnchorLongitude.Should().BeApproximately(_stops.Average(stop => stop.Longitude), 1e-6);
    }

    private async Task The_Stored_Stops_Are_In_Walk_Order()
    {
        var stored = await RoutePersistenceProvider.GetByName(_routeName);
        var stops = await RoutePersistenceProvider.GetStops(stored.Id);

        stops.Select(stop => stop.ShopId).Should().Equal(_stops.Select(stop => stop.Id));
        stops.Select(stop => stop.Position).Should().Equal(Enumerable.Range(0, _stops.Length));
    }

    private async Task No_Route_Is_Stored()
    {
        var count = await RoutePersistenceProvider.CountByName(_routeName);

        count.Should().Be(0);
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = EndpointPath
        }, options => options.Excluding(details => details.Extensions));
    }

    private async Task The_Response_Is_A_Validation_Error_For(string field, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        problem.Should().BeEquivalentTo(new HttpValidationProblemDetails(new Dictionary<string, string[]>
        {
            [field] = [message]
        })
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = ValidationTitle
        }, options => options
            .Excluding(details => details.Extensions)
            .Excluding(details => details.Type));
    }

    private CoordinateRequest StopAt(int position) => new()
    {
        Latitude = _stops[position].Latitude,
        Longitude = _stops[position].Longitude,
    };

    private CreateRouteRequest SaveTheStops(string niche) => new()
    {
        Name = _routeName,
        Tag = _routeTag,
        Niche = niche,
        Stops = _stops.Select(stop => stop.Id).ToArray(),
        Polyline = Enumerable.Range(0, _stops.Length).Select(StopAt).ToArray(),
        DistanceMeters = _distanceMeters,
        TotalMinutes = _totalMinutes,
    };
}
