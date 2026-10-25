namespace N1che.Api.Endpoints.Shops;

internal static class ShopEndpointBuilder
{
    internal static IEndpointRouteBuilder AddShopEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("shops")
            .AddGetShopsEndpoints()
            .AddGetShopInteractionsEndpoints()
            .AddGetShopReviewsEndpoints()
            .AddCreateShopEndpoints()
            .AddShopVoteEndpoints()
            .AddShopBookmarkEndpoints()
            .RequireAuthorization()
            .WithTags("shops");

        return endpoints;
    }
}
