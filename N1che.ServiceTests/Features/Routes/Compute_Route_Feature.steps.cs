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
using N1che.ServiceTests.Infrastructure.Google;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Compute_Route_Feature : FeatureFixture
{
    private const string EndpointPath = "/routes/compute";
    private const string SuccessLog = "Route computed";
    private const string GoogleUnavailableMessage = "Google Routes is currently unavailable.";
    private const string NoWalkMessage = "No walking route runs through the requested stops.";
    private const string ValidationTitle = "One or more validation errors occurred.";
    private const string UnknownManeuver = "unknown";

    // The modes are spelled out rather than taken from the domain, so a rename has to reach the wire
    // before these pass.
    private const string YouMode = "you";
    private const string LoopMode = "loop";
    private const string UnrecognisedMode = "drive";

    private const string ModeField = "Mode";
    private const string StopsField = "Stops";
    private const string OriginLatitudeField = "Origin.Latitude";
    private const string OriginLongitudeField = "Origin.Longitude";

    private const string ModeValidationMessage = "Mode must be 'you' or 'loop'.";
    private const string StopRequiredValidationMessage = "At least one stop is required.";
    private const string TooManyStopsValidationMessage = "A route can hold at most 15 stops.";
    private const string RepeatedStopValidationMessage = "A stop can only appear once in a route.";
    private const string LoopStopsValidationMessage = "A loop needs at least two stops.";
    private const string OriginlessStopsValidationMessage = "A route without an origin needs at least two stops.";
    private const string LatitudeValidationMessage = "Latitude must be between -90 and 90.";
    private const string LongitudeValidationMessage = "Longitude must be between -180 and 180.";

    private const int TooManyStops = 16;

    private const double OutOfRangeLatitude = 200;
    private const double OutOfRangeLongitude = 200;

    private const double SecondsPerMinute = 60;

    // Offsets in degrees of latitude from this scenario's origin — each stop roughly 111m past the last.
    private static readonly double[] StopOffsets = [0.001, 0.002, 0.003];

    // Only logs written through the logger that opened the scope carry it, so the client's own error —
    // raised beneath the endpoint — does not.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // A fresh origin per scenario keeps each scenario's stubbed waypoints out of every other's.
    private readonly double _originLatitude;
    private readonly double _originLongitude;
    private readonly CoordinateResponse _origin;
    private readonly CoordinateRequest _originRequest;

    // The stop nothing seeds, so a request carrying it reaches the store and finds no shop.
    private readonly Guid _unknownStopId = Guid.NewGuid();
    private readonly string _notFoundMessage;

    // The requests Google is never asked about, each rejected before the walk is computed.
    private readonly ComputeRouteRequest _unknownStopRequest;
    private readonly ComputeRouteRequest _unrecognisedModeRequest;
    private readonly ComputeRouteRequest _stopLessRequest;
    private readonly ComputeRouteRequest _tooManyStopsRequest;
    private readonly ComputeRouteRequest _repeatedStopRequest;
    private readonly ComputeRouteRequest _singleStopLoopRequest;
    private readonly ComputeRouteRequest _originlessSingleStopRequest;
    private readonly ComputeRouteRequest _outOfRangeLatitudeRequest;
    private readonly ComputeRouteRequest _outOfRangeLongitudeRequest;

    private ShopEntity[] _stops = [];
    private GoogleRoute _googleRoute = null!;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;
    private string _googleFailureLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    public Compute_Route_Feature()
    {
        _fixture = new Fixture();
        _originLatitude = Random.Shared.Next(-80, 80) + Random.Shared.NextDouble();
        _originLongitude = Random.Shared.Next(-170, 170) + Random.Shared.NextDouble();

        _origin = new CoordinateResponse { Latitude = _originLatitude, Longitude = _originLongitude };
        _originRequest = new CoordinateRequest { Latitude = _originLatitude, Longitude = _originLongitude };

        _notFoundMessage = $"Shop {_unknownStopId} not found";

        _unknownStopRequest = new ComputeRouteRequest
        {
            Stops = [_unknownStopId, Guid.NewGuid()],
            Mode = YouMode
        };

        _unrecognisedModeRequest = _unknownStopRequest with { Mode = UnrecognisedMode };
        _stopLessRequest = _unknownStopRequest with { Stops = [], Origin = _originRequest };
        _tooManyStopsRequest = _unknownStopRequest with
        {
            Stops = Enumerable.Range(0, TooManyStops).Select(_ => Guid.NewGuid()).ToArray()
        };
        _repeatedStopRequest = _unknownStopRequest with { Stops = [_unknownStopId, _unknownStopId] };
        _singleStopLoopRequest = _unknownStopRequest with { Stops = [_unknownStopId], Mode = LoopMode };
        _originlessSingleStopRequest = _unknownStopRequest with { Stops = [_unknownStopId] };
        _outOfRangeLatitudeRequest = _unknownStopRequest with
        {
            Origin = _originRequest with { Latitude = OutOfRangeLatitude }
        };
        _outOfRangeLongitudeRequest = _unknownStopRequest with
        {
            Origin = _originRequest with { Longitude = OutOfRangeLongitude }
        };
    }

    // The walk Google reports. Every distance and duration is drawn fresh, so no number the response
    // carries can be one the service invented rather than one Google sent.
    private static double RandomMeters() => Random.Shared.Next(50, 2000) + Random.Shared.NextDouble();

    private static int RandomSeconds() => Random.Shared.Next(30, 1800);

    private async Task Shops_Exist(int count)
    {
        _stops = StopOffsets.Take(count)
            .Select(offset => ShopEntityBuilder.Build(_fixture,
                latitude: _originLatitude + offset,
                longitude: _originLongitude,
                niches: NicheConstants.Goth))
            .ToArray();

        await ShopPersistenceProvider.Insert(_stops);
    }

    private Task Google_Returns_A_Walk_Through(params CoordinateResponse[] waypoints)
    {
        _googleRoute = new GoogleRoute
        {
            DistanceMeters = RandomMeters(),
            DurationSeconds = RandomSeconds(),
            Polyline = waypoints,
            Legs = waypoints.SkipLast(1).Select((waypoint, index) => new GoogleLeg
            {
                DistanceMeters = RandomMeters(),
                DurationSeconds = RandomSeconds(),
                Polyline = [waypoint, waypoints[index + 1]],
                Steps =
                [
                    new GoogleStep
                    {
                        DistanceMeters = RandomMeters(),
                        DurationSeconds = RandomSeconds(),
                        Polyline = [waypoint, waypoints[index + 1]],
                        Instruction = $"Walk to stop {index + 1}",
                        Maneuver = "TURN_LEFT"
                    }
                ]
            }).ToArray()
        };

        GoogleRoutesMock.ReturnsRoute(waypoints, _googleRoute);
        return Task.CompletedTask;
    }

    // Google leaves the instruction out of a step that has none, which is not an empty one.
    private Task Google_Returns_A_Walk_Whose_Step_Has_No_Instruction(params CoordinateResponse[] waypoints)
    {
        _googleRoute = new GoogleRoute
        {
            DistanceMeters = RandomMeters(),
            DurationSeconds = RandomSeconds(),
            Polyline = waypoints,
            Legs =
            [
                new GoogleLeg
                {
                    DistanceMeters = RandomMeters(),
                    DurationSeconds = RandomSeconds(),
                    Polyline = waypoints,
                    Steps =
                    [
                        new GoogleStep
                        {
                            DistanceMeters = RandomMeters(),
                            DurationSeconds = RandomSeconds(),
                            Polyline = waypoints
                        }
                    ]
                }
            ]
        };

        GoogleRoutesMock.ReturnsRoute(waypoints, _googleRoute);
        return Task.CompletedTask;
    }

    private Task Google_Routes_Nothing_Through(params CoordinateResponse[] waypoints)
    {
        GoogleRoutesMock.ReturnsNoRoutes(waypoints);
        return Task.CompletedTask;
    }

    private Task Google_Answers_Unsuccessfully(params CoordinateResponse[] waypoints)
    {
        GoogleRoutesMock.AnswersUnsuccessfully(waypoints);
        _googleFailureLog =
            $"Google Routes answered {(int)GoogleRoutesMock.RejectedApiKeyStatus} for {waypoints.Length} waypoints: " +
            GoogleRoutesMock.ErrorBody;

        return Task.CompletedTask;
    }

    private async Task ComputeRoute_Is_Called(ComputeRouteRequest request)
    {
        _endpointLog = $"Computing {request.Mode} route through {request.Stops.Count} stops";
        _scopeValues = new Dictionary<string, object>
        {
            ["Mode"] = request.Mode,
            ["StopCount"] = request.Stops.Count
        };

        _response = await Client.ComputeRoute(request);
    }

    // Each leg is carried by the stop it arrives at, so the expectation walks the legs the way the
    // service does: from an origin leg i reaches stop i, without one leg i reaches stop i + 1, and a
    // loop's last leg lands back at the stop it began at.
    private async Task The_Walk_Is_Returned(bool fromOrigin = false, bool isLoop = false)
    {
        var legs = _googleRoute.Legs.ToArray();
        var arrivingLegs = new GoogleLeg?[_stops.Length];

        for (var legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            var arrivingStop = isLoop
                ? (legIndex + 1) % _stops.Length
                : legIndex + (fromOrigin ? 0 : 1);

            if (arrivingStop < _stops.Length)
            {
                arrivingLegs[arrivingStop] = legs[legIndex];
            }
        }

        var route = await _response.Content.ReadFromJsonAsync<RouteShapeResponse>();

        route.Should().BeEquivalentTo(new RouteShapeResponse
        {
            Stops = _stops.Select((stop, position) => new RouteStopResponse
            {
                Id = stop.Id.ToString(),
                Name = stop.Name,
                Address = stop.Address,
                Latitude = stop.Latitude,
                Longitude = stop.Longitude,
                PlaceStatus = stop.PlaceStatus,
                Leg = arrivingLegs[position] is not { } leg
                    ? null
                    : new RouteLegResponse
                    {
                        DistanceMeters = leg.DistanceMeters,
                        DurationSeconds = leg.DurationSeconds,
                        Polyline = leg.Polyline,
                        Steps = leg.Steps.Select(step => new RouteStepResponse
                        {
                            DistanceMeters = step.DistanceMeters,
                            DurationSeconds = step.DurationSeconds,
                            Polyline = step.Polyline,
                            Instruction = step.Instruction ?? string.Empty,
                            Maneuver = step.Maneuver ?? UnknownManeuver,
                        }).ToArray()
                    },
            }).ToArray(),
            Polyline = _googleRoute.Polyline,
            DistanceMeters = _googleRoute.DistanceMeters,
            TotalMinutes = (int)Math.Round(_googleRoute.DurationSeconds / SecondsPerMinute),
            Mode = isLoop ? LoopMode : YouMode,
        }, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>());
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

    private CoordinateResponse StopAt(int position) => new()
    {
        Latitude = _stops[position].Latitude,
        Longitude = _stops[position].Longitude,
    };

    private ComputeRouteRequest WalkThroughTheStops(string mode, bool fromOrigin = false) => new()
    {
        Stops = _stops.Select(stop => stop.Id).ToArray(),
        Origin = fromOrigin
            ? new CoordinateRequest { Latitude = _originLatitude, Longitude = _originLongitude }
            : null,
        Mode = mode,
    };
}
