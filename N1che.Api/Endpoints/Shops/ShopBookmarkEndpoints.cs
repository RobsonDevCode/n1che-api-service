using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Domain.Interfaces.Services.Shops;

namespace N1che.Api.Endpoints.Shops;

internal static class ShopBookmarkEndpoints
{
    internal static RouteGroupBuilder AddShopBookmarkEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/save", Add)
            .WithSummary("Save Shop")
            .WithDescription("Bookmark a shop on the calling user's behalf");

        group.MapDelete("{id:guid}/save", Remove)
            .WithSummary("Unsave Shop")
            .WithDescription("Withdraw the calling user's bookmark of a shop");

        return group;
    }

    private static async Task<NoContent> Add(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IShopBookmarkingService shopBookmarkingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Save Shop");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Bookmarking shop {ShopId}", id);

        await shopBookmarkingService.AddAsync(id, user, cancellationToken);

        logger.LogInformation("Shop bookmarked");

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> Remove(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IShopBookmarkingService shopBookmarkingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Unsave Shop");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Removing bookmark of shop {ShopId}", id);

        await shopBookmarkingService.RemoveAsync(id, user, cancellationToken);

        logger.LogInformation("Shop bookmark removed");

        return TypedResults.NoContent();
    }
}
