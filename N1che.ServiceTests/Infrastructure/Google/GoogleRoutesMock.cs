using System.Net;
using N1che.Contracts.Response.Routes;
using WireMock.Matchers;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace N1che.ServiceTests.Infrastructure.Google;

// Stands in for routes.googleapis.com: the app is pointed at this server through GoogleRoutes__BaseUrl,
// so the real GoogleRoutesClient makes real HTTP calls. The waypoints are part of the match, so a
// client that reorders them, drops the origin or fails to close a loop gets no stubbed route.
internal static class GoogleRoutesMock
{
    internal const string ApiKey = "test-google-routes-key";

    // Google serves a key it will not accept with this.
    internal const HttpStatusCode RejectedApiKeyStatus = HttpStatusCode.Forbidden;

    internal const string ErrorBody =
        """{"error":{"code":403,"message":"API key not valid","status":"PERMISSION_DENIED"}}""";

    private const string ApiKeyHeader = "X-Goog-Api-Key";
    private const string FieldMaskHeader = "X-Goog-FieldMask";

    private const string ComputeRoutesPath = "/directions/v2:computeRoutes";

    private const string FieldMask =
        "routes.distanceMeters,routes.duration,routes.polyline," +
        "routes.legs.distanceMeters,routes.legs.duration,routes.legs.polyline," +
        "routes.legs.steps.distanceMeters,routes.legs.steps.staticDuration," +
        "routes.legs.steps.polyline,routes.legs.steps.navigationInstruction";

    private static WireMockServer? s_server;

    private static WireMockServer Server =>
        s_server ?? throw new InvalidOperationException("Google Routes mock has not been started");

    internal static string Start()
    {
        s_server = WireMockServer.Start();
        return s_server.Url!;
    }

    internal static void Stop() => s_server?.Stop();

    // A duration is protobuf seconds and a GeoJSON coordinate is longitude first, both as Google sends
    // them rather than as we hold them.
    internal static void ReturnsRoute(IReadOnlyCollection<CoordinateResponse> waypoints, GoogleRoute route)
    {
        Server
            .Given(RequestFor(waypoints))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new
                {
                    routes = new[]
                    {
                        new
                        {
                            distanceMeters = route.DistanceMeters,
                            duration = $"{route.DurationSeconds}s",
                            polyline = new
                            {
                                geoJsonLinestring = new
                                {
                                    coordinates = route.Polyline
                                        .Select(point => new[] { point.Longitude, point.Latitude }).ToArray()
                                }
                            },
                            legs = route.Legs.Select(leg => new
                            {
                                distanceMeters = leg.DistanceMeters,
                                duration = $"{leg.DurationSeconds}s",
                                polyline = new
                                {
                                    geoJsonLinestring = new
                                    {
                                        coordinates = leg.Polyline
                                            .Select(point => new[] { point.Longitude, point.Latitude }).ToArray()
                                    }
                                },
                                steps = leg.Steps.Select(step => new
                                {
                                    distanceMeters = step.DistanceMeters,
                                    staticDuration = $"{step.DurationSeconds}s",
                                    polyline = new
                                    {
                                        geoJsonLinestring = new
                                        {
                                            coordinates = step.Polyline
                                                .Select(point => new[] { point.Longitude, point.Latitude }).ToArray()
                                        }
                                    },
                                    navigationInstruction = step.Instruction is null && step.Maneuver is null
                                        ? null
                                        : new { instructions = step.Instruction, maneuver = step.Maneuver }
                                }).ToArray()
                            }).ToArray()
                        }
                    }
                }));
    }

    // Google omits the field entirely when it could route nothing, rather than sending an empty array.
    internal static void ReturnsNoRoutes(IReadOnlyCollection<CoordinateResponse> waypoints)
    {
        Server
            .Given(RequestFor(waypoints))
            .RespondWith(Response.Create().WithStatusCode(HttpStatusCode.OK).WithBodyAsJson(new { }));
    }

    internal static void AnswersUnsuccessfully(IReadOnlyCollection<CoordinateResponse> waypoints)
    {
        Server
            .Given(RequestFor(waypoints))
            .RespondWith(Response.Create().WithStatusCode(RejectedApiKeyStatus).WithBody(ErrorBody));
    }

    private static IRequestBuilder RequestFor(IReadOnlyCollection<CoordinateResponse> waypoints) =>
        Request.Create()
            .WithPath(ComputeRoutesPath)
            .UsingPost()
            .WithHeader(ApiKeyHeader, ApiKey)
            .WithHeader(FieldMaskHeader, FieldMask)
            .WithBody(new JsonPartialMatcher(new
            {
                origin = new
                {
                    location = new
                    {
                        latLng = new { latitude = waypoints.First().Latitude, longitude = waypoints.First().Longitude }
                    }
                },
                destination = new
                {
                    location = new
                    {
                        latLng = new { latitude = waypoints.Last().Latitude, longitude = waypoints.Last().Longitude }
                    }
                },
                intermediates = waypoints.Skip(1).SkipLast(1).Select(waypoint => new
                {
                    location = new
                    {
                        latLng = new { latitude = waypoint.Latitude, longitude = waypoint.Longitude }
                    }
                }).ToArray(),
                travelMode = "WALK",
                polylineEncoding = "GEO_JSON_LINESTRING"
            }));
}
