using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;

namespace N1che.Api.OpenApi;

internal static class SchemaExamples
{
    internal static OpenApiOptions AddSchemaExamples(
        this OpenApiOptions options,
        IReadOnlyDictionary<Type, Func<JsonNode>> examples) =>
        options.AddSchemaTransformer((schema, context, _) =>
        {
            if (examples.TryGetValue(context.JsonTypeInfo.Type, out var example))
            {
                schema.Example = example();
            }

            return Task.CompletedTask;
        });
}
