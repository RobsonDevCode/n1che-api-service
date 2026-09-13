namespace N1che.Api.Endpoints.Routes;

internal static class RouteEndpointBuilder
{
    internal static IEndpointRouteBuilder AddRouteEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("routes")
            .AddGetRoutesEndpoints()
            .RequireAuthorization()
            .WithTags("routes");

        return endpoints;
    }
}
