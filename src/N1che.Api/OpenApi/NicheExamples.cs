using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Response.Niches;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the niches endpoint.</summary>
internal static class NicheExamples
{
    internal static OpenApiOptions AddNicheExamples(this OpenApiOptions options) =>
        options.AddSchemaExamples(new Dictionary<Type, Func<JsonNode>>
        {
            [typeof(NicheResponse)] = Niche
        });

    private static JsonNode Niche() => new JsonObject
    {
        ["id"] = "goth",
        ["label"] = "Goth",
        ["subLabel"] = "dark",
        ["description"] = "Dark aesthetics, velvet, silver hardware"
    };
}
