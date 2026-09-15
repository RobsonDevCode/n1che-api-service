using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Api.Extensions.Shops;
using N1che.Contracts.Response.Shops;
using N1che.Domain.Interfaces.Services.Shops;

namespace N1che.Api.Endpoints.Shops;

internal static class GetShopInteractionsEndpoints
{
    internal static RouteGroupBuilder AddGetShopInteractionsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}/interactions", GetByShopId)
            .WithSummary("Get Shop Interactions")
            .WithDescription("Get the shop's live vote count with the calling user's own vote and bookmark state");

        return group;
    }

    private static async Task<Ok<ShopInteractionsResponse>> GetByShopId(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IShopInteractionsRetrievalService shopInteractionsRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Get Shop Interactions");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Getting interactions for shop {ShopId}", id);

        var interactions = await shopInteractionsRetrievalService.GetByShopIdAsync(id, user.Id, cancellationToken);

        logger.LogInformation("Shop interactions retrieved");

        return TypedResults.Ok(interactions.ToResponse());
    }
}
