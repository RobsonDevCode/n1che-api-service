namespace N1che.Api.Endpoints.Niches;

internal static class NicheEndpointBuilder
{
    internal static IEndpointRouteBuilder AddNicheEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("niches")
            .AddGetNichesEndpoints()
            .RequireAuthorization()
            .WithTags("niches");

        return endpoints;
    }
}
