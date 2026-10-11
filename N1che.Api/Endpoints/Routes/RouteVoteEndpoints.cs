using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Domain.Interfaces.Services.Routes;

namespace N1che.Api.Endpoints.Routes;

internal static class RouteVoteEndpoints
{
    internal static RouteGroupBuilder AddRouteVoteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/vote", Add)
            .WithSummary("Add Route Vote")
            .WithDescription("Upvote a route on the calling user's behalf");

        group.MapDelete("{id:guid}/vote", Remove)
            .WithSummary("Remove Route Vote")
            .WithDescription("Withdraw the calling user's upvote of a route");

        return group;
    }

    private static async Task<NoContent> Add(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IRouteVotingService routeVotingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Add Route Vote");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["RouteId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Adding vote for route {RouteId}", id);

        await routeVotingService.AddAsync(id, user, cancellationToken);

        logger.LogInformation("Route vote added");

        return TypedResults.NoContent();
    }

    private static async Task<NoContent> Remove(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IRouteVotingService routeVotingService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Remove Route Vote");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["RouteId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Removing vote for route {RouteId}", id);

        await routeVotingService.RemoveAsync(id, user, cancellationToken);

        logger.LogInformation("Route vote removed");

        return TypedResults.NoContent();
    }
}
