using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Api.Extensions.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Domain.Interfaces.Services.Routes;

namespace N1che.Api.Endpoints.Routes;

internal static class GetRouteInteractionsEndpoints
{
    internal static RouteGroupBuilder AddGetRouteInteractionsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}/interactions", GetByRouteId)
            .WithSummary("Get Route Interactions")
            .WithDescription("Get the route's live vote count with the calling user's own vote state");

        return group;
    }

    private static async Task<Ok<RouteInteractionsResponse>> GetByRouteId(
        [FromRoute] Guid id,
        ClaimsPrincipal principal,
        [FromServices] IRouteInteractionsRetrievalService routeInteractionsRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Get Route Interactions");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["RouteId"] = id,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Getting interactions for route {RouteId}", id);

        var interactions = await routeInteractionsRetrievalService.GetByRouteIdAsync(id, user.Id, cancellationToken);

        logger.LogInformation("Route interactions retrieved");

        return TypedResults.Ok(interactions.ToResponse());
    }
}
