using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Requests.Routes;
using N1che.Contracts.Response.Routes;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the routes endpoints.</summary>
internal static class RouteExamples
{
    private const string FirstStopId = "0f5f3a1e-6f1a-4a2e-9c3d-2b6a1c4d5e6f";
    private const string SecondStopId = "1a2b3c4d-5e6f-4a7b-8c9d-0e1f2a3b4c5d";
    private const string ThirdStopId = "2b3c4d5e-6f7a-4b8c-9d0e-1f2a3b4c5d6e";

    internal static OpenApiOptions AddRouteExamples(this OpenApiOptions options) =>
        options.AddSchemaTransformer((schema, context, _) =>
        {
            if (context.JsonTypeInfo.Type == typeof(CreateRouteRequest))
            {
                schema.Example = SaveRouteRequest();
            }
            else if (context.JsonTypeInfo.Type == typeof(CreatedRouteResponse))
            {
                schema.Example = CreatedRoute();
            }
            else if (context.JsonTypeInfo.Type == typeof(RouteResponse))
            {
                schema.Example = SavedRoute();
            }

            return Task.CompletedTask;
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

    private static JsonNode CreatedRoute() => new JsonObject
    {
        ["id"] = "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ["createdAt"] = "2026-09-27T09:14:22.481Z"
    };

    // A saved route carries no legs: only the endpoints that compute a walk report them.
    private static JsonNode SavedRoute() => new JsonObject
    {
        ["id"] = "7c9e6679-7425-40de-944b-e07fc1f90ae7",
        ["name"] = "Northern Quarter Crawl",
        ["tag"] = "after dark",
        ["niche"] = "goth",
        ["createdBy"] = "velvetghoul",
        ["userId"] = "a4f1c2d3-9b8e-4c7a-86d5-1e2f3a4b5c6d",
        ["stops"] = new JsonArray
        {
            Stop(FirstStopId, "Void & Velvet", "14 Tib Street, Manchester M4 1SH", 53.4842, -2.2364),
            Stop(SecondStopId, "The Crypt", "3 Hilton Street, Manchester M1 2EF", 53.4831, -2.2352),
            Stop(ThirdStopId, "Séance Supply", "27 Oldham Street, Manchester M1 1JG", 53.4819, -2.2338)
        },
        ["polyline"] = WalkPolyline(),
        ["distanceMeters"] = 412.7,
        ["totalMinutes"] = 6,
        ["totalUpvotes"] = 38,
        ["createdAt"] = "2026-09-27T09:14:22.481Z"
    };

    private static JsonArray WalkPolyline() =>
    [
        Coordinate(53.4842, -2.2364),
        Coordinate(53.4836, -2.2359),
        Coordinate(53.4831, -2.2352),
        Coordinate(53.4819, -2.2338)
    ];

    private static JsonNode Stop(string id, string name, string address, double latitude, double longitude) =>
        new JsonObject
        {
            ["id"] = id,
            ["name"] = name,
            ["address"] = address,
            ["latitude"] = latitude,
            ["longitude"] = longitude,
            ["placeStatus"] = "operational",
            ["leg"] = null
        };

    private static JsonNode Coordinate(double latitude, double longitude) => new JsonObject
    {
        ["latitude"] = latitude,
        ["longitude"] = longitude
    };
}
