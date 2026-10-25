using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Response.Places;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the places endpoints.</summary>
internal static class PlaceExamples
{
    private const string GooglePlaceId = "ChIJ2eUgeAK6j4ARbn5u_wAGqWA";

    internal static OpenApiOptions AddPlaceExamples(this OpenApiOptions options) =>
        options.AddSchemaExamples(new Dictionary<Type, Func<JsonNode>>
        {
            [typeof(PlaceResponse)] = Place,
            [typeof(PlacePhotoMediaResponse)] = PhotoMedia
        });

    private static JsonNode Place() => new JsonObject
    {
        ["googlePlaceId"] = GooglePlaceId,
        ["name"] = "Void & Velvet",
        ["address"] = "14 Tib Street, Manchester M4 1SH",
        ["latitude"] = 53.4842,
        ["longitude"] = -2.2364,
        ["photo"] = new JsonObject
        {
            ["reference"] = $"places/{GooglePlaceId}/photos/AWYs27zQ8vR1mJkLqNpXo",
            ["googleMapsUri"] = "https://maps.google.com/maps/place//data=!3m4!1e2!3m2!1sAWYs27zQ8vR1mJkLqNpXo",
            ["authorAttributions"] = new JsonArray
            {
                new JsonObject
                {
                    ["displayName"] = "Mara Quinn",
                    ["uri"] = "https://maps.google.com/maps/contrib/104851239687410235671",
                    ["photoUri"] = "https://lh3.googleusercontent.com/a-/ALV-UjXq9Fz2"
                }
            }
        }
    };

    private static JsonNode PhotoMedia() => new JsonObject
    {
        ["photoUri"] = "https://lh3.googleusercontent.com/place-photos/AJnk2cy7Rk3vQ0x=s4800-w800"
    };
}
