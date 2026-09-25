using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Domain.Interfaces.Services.Shops;

namespace N1che.Api.Endpoints.Shops;

internal static class ShopVoteEndpoints
{
    internal static RouteGroupBuilder AddShopVoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/vote", Add)
            .WithSummary("Add Shop Vote")
            .WithDescription("Upvote a shop on the calling user's behalf");

        group.MapDelete("{id:guid}/vote", Remove)
            .WithSummary("Remove Shop Vote")
            .WithDescription("Withdraw the calling user's upvote of a shop");

        return group;
    }

    private static async Task<NoContent> Add(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IShopVotingService shopVotingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Add Shop Vote");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Adding vote for shop {ShopId}", id);

        await shopVotingService.AddAsync(id, user, cancellationToken);

        logger.LogInformation("Shop vote added");

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> Remove(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IShopVotingService shopVotingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Remove Shop Vote");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Removing vote for shop {ShopId}", id);

        await shopVotingService.RemoveAsync(id, user, cancellationToken);

        logger.LogInformation("Shop vote removed");

        return TypedResults.NoContent();
    }
}
