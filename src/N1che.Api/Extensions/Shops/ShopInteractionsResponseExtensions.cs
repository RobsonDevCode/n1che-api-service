using N1che.Contracts.Response.Shops;
using N1che.Domain.Models.Shops;

namespace N1che.Api.Extensions.Shops;

public static class ShopInteractionsResponseExtensions
{
    public static ShopInteractionsResponse ToResponse(this ShopInteractionsModel interactions) => new()
    {
        VoteCount = interactions.VoteCount,
        Voted = interactions.Voted,
        Saved = interactions.Saved,
    };
}
