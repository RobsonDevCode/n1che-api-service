using Microsoft.Extensions.Caching.Memory;
using N1che.Domain.Exceptions;
using N1che.Domain.Extensions;
using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.Services.Routes;

public sealed class RouteRetrievalService : IRouteRetrievalService
{
    // How far a walk to the first stop is reasonable; a route can appear in /routes and still be further.
    private const double MaxOriginDistanceMeters = 5000;

    private readonly IRoutesReader _routesReader;
    private readonly IRouteComputationService _routeComputationService;
    private readonly IMemoryCache _cache;

    public RouteRetrievalService(
        IRoutesReader routesReader,
        IRouteComputationService routeComputationService,
        IMemoryCache cache)
    {
        _routesReader = routesReader;
        _routeComputationService = routeComputationService;
        _cache = cache;
    }

    public Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearbyAsync(RoutesFilterModel filterModel, CancellationToken cancellationToken)
    {
        return _routesReader.GetTopRatedNearby(filterModel, cancellationToken);
    }

    public async ValueTask<RouteDetailModel> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        // Throwing inside the factory leaves the entry uncommitted, so unknown ids are never cached.
        var route = await _cache.GetOrCreateAsync($"route:{id}", async entry =>
        {
            entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(2));
            return await _routesReader.GetById(id, cancellationToken)
                   ?? throw new NotFoundException(EntityTypes.Route, id);
        });

        return route!;
    }

    public async Task<ComputedRouteModel> GetComputedRouteAsync(Guid id, CoordinateModel origin, CancellationToken cancellationToken)
    {
        var route = await GetByIdAsync(id, cancellationToken);

        var firstStop = route.Stops.First();
        var originDistanceMeters = origin.DistanceMetersTo(new CoordinateModel
        {
            Latitude = firstStop.Latitude,
            Longitude = firstStop.Longitude,
        });

        if (originDistanceMeters > MaxOriginDistanceMeters)
        {
            throw new RouteException(EntityTypes.Route, id, MaxOriginDistanceMeters);
        }

        var walk = await _routeComputationService.ComputeAsync(new ComputeRouteModel
        {
            StopIds = route.Stops.Select(stop => stop.Id).ToArray(),
            Origin = origin,
            Mode = RouteModes.You,
        }, cancellationToken);

        return new ComputedRouteModel
        {
            Detail = route,
            Walk = walk,
        };
    }
}
