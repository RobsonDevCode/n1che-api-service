using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Requests.Routes;
using N1che.Contracts.Response.Routes;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the routes endpoints.</summary>
internal static class RouteExamples
{
    private const string RouteId = "7c9e6679-7425-40de-944b-e07fc1f90ae7";
    private const string FirstStopId = "0f5f3a1e-6f1a-4a2e-9c3d-2b6a1c4d5e6f";
    private const string SecondStopId = "1a2b3c4d-5e6f-4a7b-8c9d-0e1f2a3b4c5d";
    private const string ThirdStopId = "2b3c4d5e-6f7a-4b8c-9d0e-1f2a3b4c5d6e";
    private const string CreatedAt = "2026-09-27T09:14:22.481Z";

    internal static OpenApiOptions AddRouteExamples(this OpenApiOptions options) =>
        options.AddSchemaExamples(new Dictionary<Type, Func<JsonNode>>
        {
            [typeof(CreateRouteRequest)] = SaveRouteRequest,
            [typeof(ComputeRouteRequest)] = ComputeRouteRequest,
            [typeof(CreatedRouteResponse)] = CreatedRoute,
            [typeof(RouteResponse)] = SavedRoute,
            [typeof(ComputedRouteResponse)] = ComputedRoute,
            [typeof(RouteShapeResponse)] = RouteShape,
            [typeof(RouteInteractionsResponse)] = RouteInteractions
        });

    private static JsonNode SaveRouteRequest() => new JsonObject
    {
        ["name"] = "Northern Quarter Crawl",
        ["tag"] = "after dark",
        ["niche"] = "goth",
        ["stops"] = new JsonArray(FirstStopId, SecondStopId, ThirdStopId),
        ["polyline"] = WalkPolyline(),
        ["distanceMeters"] = 412.7,
        ["totalMinutes"] = 6
    };

    private static JsonNode ComputeRouteRequest() => new JsonObject
    {
        ["stops"] = new JsonArray(FirstStopId, SecondStopId, ThirdStopId),
        ["origin"] = Coordinate(53.4808, -2.2374),
        ["mode"] = "you"
    };

    private static JsonNode CreatedRoute() => new JsonObject
    {
        ["id"] = RouteId,
        ["createdAt"] = CreatedAt
    };

    // A saved route carries no legs: only the endpoints that compute a walk report them.
    private static JsonNode SavedRoute() => new JsonObject
    {
        ["id"] = RouteId,
        ["name"] = "Northern Quarter Crawl",
        ["tag"] = "after dark",
        ["niche"] = "goth",
        ["createdBy"] = "velvetghoul",
        ["userId"] = "a4f1c2d3-9b8e-4c7a-86d5-1e2f3a4b5c6d",
        ["stops"] = new JsonArray
        {
            FirstStop(leg: null),
            SecondStop(leg: null),
            ThirdStop(leg: null)
        },
        ["polyline"] = WalkPolyline(),
        ["distanceMeters"] = 412.7,
        ["totalMinutes"] = 6,
        ["totalUpvotes"] = 38,
        ["createdAt"] = CreatedAt
    };

    private static JsonNode ComputedRoute() => new JsonObject
    {
        ["id"] = RouteId,
        ["name"] = "Northern Quarter Crawl",
        ["tag"] = "after dark",
        ["niche"] = "goth",
        ["createdBy"] = "velvetghoul",
        ["userId"] = "a4f1c2d3-9b8e-4c7a-86d5-1e2f3a4b5c6d",
        ["stops"] = ComputedStops(),
        ["polyline"] = OriginWalkPolyline(),
        ["distanceMeters"] = 812.4,
        ["totalMinutes"] = 11,
        ["createdAt"] = CreatedAt
    };

    private static JsonNode RouteShape() => new JsonObject
    {
        ["stops"] = ComputedStops(),
        ["polyline"] = OriginWalkPolyline(),
        ["distanceMeters"] = 812.4,
        ["totalMinutes"] = 11,
        ["mode"] = "you"
    };

    private static JsonNode RouteInteractions() => new JsonObject
    {
        ["voteCount"] = 38,
        ["voted"] = true
    };

    private static JsonArray ComputedStops() =>
    [
        FirstStop(Leg(399.7, 300,
            [Coordinate(53.4808, -2.2374), Coordinate(53.4824, -2.2370), Coordinate(53.4842, -2.2364)],
            Step(180.2, 135, "Head north on Lever Street", "DEPART", [Coordinate(53.4808, -2.2374), Coordinate(53.4824, -2.2370)]),
            Step(219.5, 165, "Turn left onto Tib Street", "TURN_LEFT", [Coordinate(53.4824, -2.2370), Coordinate(53.4842, -2.2364)]))),
        SecondStop(Leg(160.3, 120,
            [Coordinate(53.4842, -2.2364), Coordinate(53.4831, -2.2352)],
            Step(160.3, 120, "Turn right onto Hilton Street", "TURN_RIGHT", [Coordinate(53.4842, -2.2364), Coordinate(53.4831, -2.2352)]))),
        ThirdStop(Leg(252.4, 190,
            [Coordinate(53.4831, -2.2352), Coordinate(53.4819, -2.2338)],
            Step(252.4, 190, "Turn left onto Oldham Street", "TURN_LEFT", [Coordinate(53.4831, -2.2352), Coordinate(53.4819, -2.2338)])))
    ];

    private static JsonNode FirstStop(JsonNode? leg) =>
        Stop(FirstStopId, "Void & Velvet", "14 Tib Street, Manchester M4 1SH", 53.4842, -2.2364, leg);

    private static JsonNode SecondStop(JsonNode? leg) =>
        Stop(SecondStopId, "The Crypt", "3 Hilton Street, Manchester M1 2EF", 53.4831, -2.2352, leg);

    private static JsonNode ThirdStop(JsonNode? leg) =>
        Stop(ThirdStopId, "Séance Supply", "27 Oldham Street, Manchester M1 1JG", 53.4819, -2.2338, leg);

    private static JsonArray WalkPolyline() =>
    [
        Coordinate(53.4842, -2.2364),
        Coordinate(53.4836, -2.2359),
        Coordinate(53.4831, -2.2352),
        Coordinate(53.4819, -2.2338)
    ];

    private static JsonArray OriginWalkPolyline() =>
    [
        Coordinate(53.4808, -2.2374),
        Coordinate(53.4824, -2.2370),
        Coordinate(53.4842, -2.2364),
        Coordinate(53.4831, -2.2352),
        Coordinate(53.4819, -2.2338)
    ];

    private static JsonNode Stop(
        string id, string name, string address, double latitude, double longitude, JsonNode? leg) =>
        new JsonObject
        {
            ["id"] = id,
            ["name"] = name,
            ["address"] = address,
            ["latitude"] = latitude,
            ["longitude"] = longitude,
            ["placeStatus"] = "operational",
            ["leg"] = leg
        };

    private static JsonNode Leg(double distanceMeters, int durationSeconds, JsonArray polyline, params JsonNode[] steps) =>
        new JsonObject
        {
            ["distanceMeters"] = distanceMeters,
            ["durationSeconds"] = durationSeconds,
            ["polyline"] = polyline,
            ["steps"] = new JsonArray(steps)
        };

    private static JsonNode Step(
        double distanceMeters, int durationSeconds, string instruction, string maneuver, JsonArray polyline) =>
        new JsonObject
        {
            ["distanceMeters"] = distanceMeters,
            ["durationSeconds"] = durationSeconds,
            ["polyline"] = polyline,
            ["instruction"] = instruction,
            ["maneuver"] = maneuver
        };

    private static JsonNode Coordinate(double latitude, double longitude) => new JsonObject
    {
        ["latitude"] = latitude,
        ["longitude"] = longitude
    };
}
