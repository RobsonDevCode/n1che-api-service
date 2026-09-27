using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Api.Extensions.Routes;
using N1che.Api.Validation;
using N1che.Contracts.Requests.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Domain.Interfaces.Services.Routes;

namespace N1che.Api.Endpoints.Routes;

internal static class CreateRouteEndpoints
{
    internal static RouteGroupBuilder AddCreateRouteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", Create)
            .WithValidation<CreateRouteRequest>()
            .WithSummary("Save Route")
            .WithDescription("Save a built route, storing the walk between its stops");

        return group;
    }

    private static async Task<Created<CreatedRouteResponse>> Create(
        [FromBody] CreateRouteRequest request,
        ClaimsPrincipal principal,
        [FromServices] IRouteCreationService routeCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Save Route");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["Niche"] = request.Niche,
            ["StopCount"] = request.Stops.Count,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Saving route through {StopCount} stops for niche {Niche}",
            request.Stops.Count, request.Niche);

        var route = await routeCreationService.CreateAsync(request.ToDomainModel(user), cancellationToken);

        logger.LogInformation("Route saved");

        return TypedResults.Created($"/routes/{route.Id}", route.ToResponse());
    }
}
