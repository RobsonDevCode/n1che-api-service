using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Filters.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Google;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Route_By_Id_Feature : FeatureFixture
{
    private const string SuccessLog = "Route retrieved";
    private const string ValidationTitle = "One or more validation errors occurred.";

    private const string LatField = "Lat";
    private const string LngField = "Lng";
    private const string LatitudeValidationMessage = "Latitude must be between -90 and 90.";
    private const string LongitudeValidationMessage = "Longitude must be between -180 and 180.";

    // Mirrors the furthest the API lets an origin sit from a route's first stop.
    private const double MaxOriginDistanceMeters = 5000;

    // Roughly 11km of latitude, well past the furthest an origin may sit from the first stop.
    private const double DistantOriginOffset = 0.1;

    private const double OutOfRangeLatitude = 200;
    private const double OutOfRangeLongitude = 200;

    private const double SecondsPerMinute = 60;

    // Offsets in degrees of latitude from the origin, each stop roughly 111m from the next. They run north
    // to south so position order is the reverse of latitude order, and a walk built in any other order
    // reaches Google with waypoints no stub matches.
    private static readonly double[] StopOffsets = [0.003, 0.002, 0.001];

    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // A fresh origin per scenario keeps each scenario's stubbed waypoints out of every other's.
    private readonly double _originLatitude;
    private readonly double _originLongitude;
    private readonly GetRouteFilter _nearbyOrigin;
    private readonly GetRouteFilter _distantOrigin;
    private readonly GetRouteFilter _outOfRangeLatitudeOrigin;
    private readonly GetRouteFilter _outOfRangeLongitudeOrigin;

    // The id nothing seeds, so a request carrying it reaches the store and finds no route.
    private readonly Guid _unknownRouteId = Guid.NewGuid();
    private readonly string _notFoundMessage;

    private RouteEntity _route = null!;
    private ShopEntity[] _stops = [];
    private GoogleRoute _googleRoute = null!;
    private Guid _requestedId;
    private string _tooFarMessage = null!;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    public Get_Route_By_Id_Feature()
    {
        _fixture = new Fixture();
        _originLatitude = Random.Shared.Latitude();
        _originLongitude = Random.Shared.Longitude();

        _nearbyOrigin = new GetRouteFilter { Lat = _originLatitude, Lng = _originLongitude };
        _distantOrigin = _nearbyOrigin with { Lat = _originLatitude - DistantOriginOffset };
        _outOfRangeLatitudeOrigin = _nearbyOrigin with { Lat = OutOfRangeLatitude };
        _outOfRangeLongitudeOrigin = _nearbyOrigin with { Lng = OutOfRangeLongitude };

        _notFoundMessage = $"Route {_unknownRouteId} not found";
    }

    private async Task The_Route_Exists()
    {
        _stops = StopOffsets
            .Select(offset => ShopEntityBuilder.Build(_fixture,
                latitude: _originLatitude + offset,
                longitude: _originLongitude,
                niches: NicheConstants.Goth))
            .ToArray();

        await ShopPersistenceProvider.Insert(_stops);

        _route = RouteEntityBuilder.Build(_fixture, _stops, NicheConstants.Goth);
        await RoutePersistenceProvider.Insert(_route, _stops);

        _tooFarMessage = string.Format(CultureInfo.InvariantCulture,
            "Route {0} starts more than {1} metres from the requested location.", _route.Id, MaxOriginDistanceMeters);
    }

    private Task Google_Returns_A_Walk_From_The_Origin_Through_The_Stops()
    {
        _googleRoute = GoogleRoutesApi.ReturnsWalkThrough(
        [
            new CoordinateResponse { Latitude = _originLatitude, Longitude = _originLongitude },
            .._stops.Select(stop => new CoordinateResponse { Latitude = stop.Latitude, Longitude = stop.Longitude })
        ]);

        return Task.CompletedTask;
    }

    private async Task GetRouteById_Is_Called(Guid id, GetRouteFilter filter)
    {
        _requestedId = id;
        _endpointLog = string.Format(CultureInfo.InvariantCulture,
            "Getting route {0} from ({1}, {2})", id, filter.Lat, filter.Lng);
        _scopeValues = new Dictionary<string, object>
        {
            ["RouteId"] = id,
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng
        };

        _response = await Client.GetRouteById(id, filter);
    }

    // The walk starts at the origin, so leg i arrives at stop i.
    private async Task The_Route_Is_Returned_With_The_Walk()
    {
        var legs = _googleRoute.Legs.ToArray();
        var route = await _response.Content.ReadFromJsonAsync<RouteDetailResponse>();

        route.Should().BeEquivalentTo(new RouteDetailResponse
        {
            Id = _route.Id,
            Name = _route.Name,
            Tag = _route.Tag,
            Niche = _route.Niche,
            CreatedBy = _route.CreatedByUsername,
            UserId = _route.CreatedByUserId,
            Stops = _stops.Select((stop, position) => new RouteStopResponse
            {
                Id = stop.Id,
                Name = stop.Name,
                Address = stop.Address,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                PlaceStatus = stop.PlaceStatus,
                Leg = new RouteLegResponse
                {
                    DistanceMeters = legs[position].DistanceMeters,
                    DurationSeconds = legs[position].DurationSeconds,
                    Polyline = legs[position].Polyline,
                    Steps = legs[position].Steps.Select(step => new RouteStepResponse
                    {
                        DistanceMeters = step.DistanceMeters,
                        DurationSeconds = step.DurationSeconds,
                        Polyline = step.Polyline,
                        Instruction = step.Instruction!,
                        Maneuver = step.Maneuver!,
                    }).ToArray()
                },
            }).ToArray(),
            Polyline = _googleRoute.Polyline,
            DistanceMeters = _googleRoute.DistanceMeters,
            TotalMinutes = (int)Math.Round(_googleRoute.DurationSeconds / SecondsPerMinute),
            CreatedAt = _route.CreatedAt,
        }, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = $"/routes/{_requestedId}"
        }, options => options.Excluding(details => details.Extensions));
    }

    private async Task The_Response_Is_A_Validation_Error_For(string field, params string[] messages)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        problem.Should().BeEquivalentTo(new HttpValidationProblemDetails(new Dictionary<string, string[]>
        {
            [field] = messages
        })
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = ValidationTitle
        }, options => options
            .Excluding(details => details.Extensions)
            .Excluding(details => details.Type));
    }
}
