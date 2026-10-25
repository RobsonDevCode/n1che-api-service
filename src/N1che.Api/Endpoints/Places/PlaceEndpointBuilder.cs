namespace N1che.Api.Endpoints.Places;

internal static class PlaceEndpointBuilder
{
    internal static IEndpointRouteBuilder AddPlaceEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("places")
            .AddSearchPlacesEndpoints()
            .AddGetPlacePhotoMediaEndpoints()
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithTags("places");

        return endpoints;
    }
}
