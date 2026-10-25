using Microsoft.Extensions.Logging;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Persistence.Writers.Routes;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.Services.Routes;

public sealed class RouteCreationService : IRouteCreationService
{
    private readonly IShopsReader _shopsReader;
    private readonly IRoutesWriter _routesWriter;
    private readonly IRouteStopsWriter _routeStopsWriter;
    private readonly ITransactionScope _transactionScope;
    private readonly ILogger<RouteCreationService> _logger;

    public RouteCreationService(
        IShopsReader shopsReader,
        IRoutesWriter routesWriter,
        IRouteStopsWriter routeStopsWriter,
        ITransactionScope transactionScope,
        ILogger<RouteCreationService> logger)
    {
        _shopsReader = shopsReader;
        _routesWriter = routesWriter;
        _routeStopsWriter = routeStopsWriter;
        _transactionScope = transactionScope;
        _logger = logger;
    }

    public async Task<CreatedRouteModel> CreateAsync(CreateRouteModel route, CancellationToken cancellationToken)
    {
        var shops = await _shopsReader.GetByIds(route.StopIds, cancellationToken);
        var shopIds = shops.Select(shop => shop.Id).ToHashSet();

        foreach (var stopId in route.StopIds)
        {
            if (shopIds.Contains(stopId))
            {
                continue;
            }

            _logger.LogWarning("Route stop {ShopId} names no shop", stopId);

            throw new NotFoundException(EntityTypes.Shop, stopId);
        }

        // A route is anchored at the centroid of its stops, which for a set of points is their mean.
        var anchor = new CoordinateModel
        {
            Latitude = shops.Average(shop => shop.Latitude),
            Longitude = shops.Average(shop => shop.Longitude),
        };

        var stopPositions = route.StopIds
            .Select((stopId, position) => (StopId: stopId, Position: position))
            .ToDictionary(stop => stop.StopId, stop => stop.Position);

        return await _transactionScope.ExecuteAsync(async token =>
        {
            var created = await _routesWriter.Create(route, anchor, token);
            await _routeStopsWriter.Create(created.Id, stopPositions, token);

            return created;
        }, cancellationToken);
    }
}
