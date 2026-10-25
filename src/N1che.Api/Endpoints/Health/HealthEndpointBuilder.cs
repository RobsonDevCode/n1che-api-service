namespace N1che.Api.Endpoints.Health;

internal static class HealthEndpointBuilder
{
    internal static IEndpointRouteBuilder AddHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("health")
            .AddGetHealthEndpoints()
            .WithTags("health");

        return endpoints;
    }
}
