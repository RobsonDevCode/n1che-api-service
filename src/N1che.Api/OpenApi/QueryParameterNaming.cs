using System.Text.Json;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace N1che.Api.OpenApi;

/// <summary>The camelCase query names clients send, in place of the filter records' property names.</summary>
internal static class QueryParameterNaming
{
    internal static OpenApiOptions AddCamelCaseQueryParameters(this OpenApiOptions options) =>
        options.AddOperationTransformer((operation, _, _) =>
        {
            var queryParameters = operation.Parameters?
                .OfType<OpenApiParameter>()
                .Where(parameter => parameter is { In: ParameterLocation.Query, Name: not null }) ?? [];

            foreach (var parameter in queryParameters)
            {
                parameter.Name = JsonNamingPolicy.CamelCase.ConvertName(parameter.Name!);
            }

            return Task.CompletedTask;
        });
}
