namespace N1che.Api.Endpoints.Places;

internal static class PlaceEndpointBuilder
{
    internal static IEndpointRouteBuilder AddPlaceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("places")
            .AddSearchPlacesEndpoints()
            .RequireAuthorization()
            .WithTags("places");

        return endpoints;
    }
}
