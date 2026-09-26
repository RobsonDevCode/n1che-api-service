using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Routes;
using N1che.Api.Validation;
using N1che.Contracts.Requests.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Domain.Interfaces.Services.Routes;

namespace N1che.Api.Endpoints.Routes;

internal static class ComputeRouteEndpoints
{
    internal static RouteGroupBuilder AddComputeRouteEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("compute", Compute)
            .WithValidation<ComputeRouteRequest>()
            .WithSummary("Compute Route")
            .WithDescription("Compute the walk through a set of shops without saving the route");

        return group;
    }

    private static async Task<Ok<RouteShapeResponse>> Compute(
        [FromBody] ComputeRouteRequest request,
        [FromServices] IRouteComputationService routeComputationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Compute Route");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["Mode"] = request.Mode,
            ["StopCount"] = request.Stops.Count
        });

        logger.LogInformation("Computing {Mode} route through {StopCount} stops", request.Mode, request.Stops.Count);

        var route = await routeComputationService.ComputeAsync(request.ToDomainModel(), cancellationToken);

        logger.LogInformation("Route computed");

        return TypedResults.Ok(route.ToResponse());
    }
}
