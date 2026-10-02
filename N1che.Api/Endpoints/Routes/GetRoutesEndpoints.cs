using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Routes;
using N1che.Api.Validation;
using N1che.Contracts.Filters.Routes;
using N1che.Contracts.Response.Routes;
using N1che.Domain.Interfaces.Services.Routes;

namespace N1che.Api.Endpoints.Routes;

internal static class GetRoutesEndpoints
{
    internal static RouteGroupBuilder AddGetRoutesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetTopRatedNearby)
            .WithValidation<RoutesFilter>()
            .WithSummary("Get Routes")
            .WithDescription("Get the top-rated routes within a radius of a location for a niche, most upvoted first");

        group.MapGet("{id:guid}", GetById)
            .WithValidation<GetRouteFilter>()
            .WithSummary("Get Route By Id")
            .WithDescription("Get a saved route with its walk computed from an origin through its stops");

        return group;
    }

    private static async Task<Ok<RouteDetailResponse>> GetById(
        [FromRoute] Guid id,
        [AsParameters] GetRouteFilter filter,
        [FromServices] IRouteRetrievalService routeRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Route By Id");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["RouteId"] = id,
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng
        });

        logger.LogInformation("Getting route {RouteId} from ({Latitude}, {Longitude})", id, filter.Lat, filter.Lng);

        var route = await routeRetrievalService.GetComputedRouteAsync(id, filter.ToDomainModel(), cancellationToken);

        logger.LogInformation("Route retrieved");

        return TypedResults.Ok(route.ToDetailResponse());
    }

    private static async Task<Ok<IReadOnlyCollection<RouteResponse>>> GetTopRatedNearby(
        [AsParameters] RoutesFilter filter,
        [FromServices] IRouteRetrievalService routeRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Routes");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng,
            ["Niche"] = filter.Niche
        });

        logger.LogInformation("Getting top rated routes at ({Latitude}, {Longitude}) for niche {Niche}",
            filter.Lat, filter.Lng, filter.Niche);

        var routes = await routeRetrievalService.GetTopRatedNearbyAsync(filter.ToDomainFilter(), cancellationToken);

        logger.LogInformation("Routes retrieved");

        return TypedResults.Ok(routes.ToResponse());
    }
}
