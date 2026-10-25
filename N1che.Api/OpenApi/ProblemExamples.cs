using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace N1che.Api.OpenApi;

/// <summary>The failure body Scalar shows under each error status code, the same for every endpoint.</summary>
internal static partial class ProblemExamples
{
    private const string ProblemContentType = "application/problem+json";
    private const string ProblemDetailsSchema = "ProblemDetails";
    private const string InternalServerError = "500";
    private const string TraceId = "00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-00";

    internal static OpenApiOptions AddProblemExamples(this OpenApiOptions options) =>
        options.AddOperationTransformer((operation, context, _) =>
        {
            operation.Responses ??= new OpenApiResponses();
            operation.Responses.TryAdd(InternalServerError, new OpenApiResponse
            {
                Description = "Internal Server Error",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    [ProblemContentType] = new()
                    {
                        Schema = new OpenApiSchemaReference(ProblemDetailsSchema, context.Document)
                    }
                }
            });

            var instance = RouteConstraint().Replace($"/{context.Description.RelativePath}", "{$1}");
            foreach (var (statusCode, response) in operation.Responses)
            {
                if (response.Content?.TryGetValue(ProblemContentType, out var content) is true
                    && content is OpenApiMediaType mediaType
                    && ProblemFor(statusCode, instance) is { } problem)
                {
                    mediaType.Example = problem;
                }
            }

            return Task.CompletedTask;
        });

    [GeneratedRegex(@"\{(\w+):[^}]+\}")]
    private static partial Regex RouteConstraint();

    private static JsonNode? ProblemFor(string statusCode, string instance) => statusCode switch
    {
        "400" => ValidationProblem(),
        "401" => Problem(StatusCodes.Status401Unauthorized, "Unauthorized", "Unauthorized", instance),
        "404" => Problem(StatusCodes.Status404NotFound, "Data not found", "Data not found", instance),
        "409" => Problem(StatusCodes.Status409Conflict,
            "An item with the same key has already been added.",
            "An item with the same key has already been added.", instance),
        "422" => Problem(StatusCodes.Status422UnprocessableEntity,
            "The request could not be processed.", "The request could not be processed.", instance),
        InternalServerError => Problem(StatusCodes.Status500InternalServerError,
            "An unexpected error occurred",
            "An unexpected error occurred. Please try again later.", instance),
        "502" => Problem(StatusCodes.Status502BadGateway,
            "A third-party service is currently unavailable.",
            "A third-party service is currently unavailable.", instance),
        "503" => NotReady(),
        _ => null
    };

    private static JsonNode Problem(int statusCode, string title, string detail, string instance) => new JsonObject
    {
        ["title"] = title,
        ["status"] = statusCode,
        ["detail"] = detail,
        ["instance"] = instance,
        ["traceId"] = TraceId
    };

    private static JsonNode ValidationProblem() => new JsonObject
    {
        ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
        ["title"] = "One or more validation errors occurred.",
        ["status"] = StatusCodes.Status400BadRequest,
        ["errors"] = new JsonObject { ["Field"] = new JsonArray("Field is invalid.") },
        ["traceId"] = TraceId
    };

    // The readiness endpoint writes this itself rather than the exception handler, so it has a type and no instance.
    private static JsonNode NotReady() => new JsonObject
    {
        ["type"] = "https://tools.ietf.org/html/rfc9110#section-15.6.4",
        ["title"] = "Service Unavailable",
        ["status"] = StatusCodes.Status503ServiceUnavailable,
        ["detail"] = "Service is not ready",
        ["traceId"] = TraceId
    };
}
