namespace N1che.Api.Endpoints.Shops;

internal static class ShopEndpointBuilder
{
    internal static IEndpointRouteBuilder AddShopEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("shops")
            .AddGetShopsEndpoints()
            .AddGetShopInteractionsEndpoints()
            .AddGetShopReviewsEndpoints()
            .AddCreateShopReviewEndpoints()
            .AddUpdateShopReviewEndpoints()
            .AddCreateShopEndpoints()
            .AddShopVoteEndpoints()
            .AddShopBookmarkEndpoints()
            .RequireAuthorization()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithTags("shops");

        return endpoints;
    }
}
