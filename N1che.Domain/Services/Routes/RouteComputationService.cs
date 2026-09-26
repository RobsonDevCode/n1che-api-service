using Microsoft.Extensions.Logging;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.Services.Routes;

public sealed class RouteComputationService : IRouteComputationService
{
    // The device allows the same, so a computed route and a saved one agree on a route's time.
    private const int BrowseMinutesPerStop = 15;

    private const double SecondsPerMinute = 60;

    private readonly IShopsReader _shopsReader;
    private readonly IGoogleRoutesClient _googleRoutesClient;
    private readonly ILogger<RouteComputationService> _logger;

    public RouteComputationService(
        IShopsReader shopsReader,
        IGoogleRoutesClient googleRoutesClient,
        ILogger<RouteComputationService> logger)
    {
        _shopsReader = shopsReader;
        _googleRoutesClient = googleRoutesClient;
        _logger = logger;
    }

    public async Task<RouteShapeModel> ComputeAsync(ComputeRouteModel route, CancellationToken cancellationToken)
    {
        var shops = await _shopsReader.GetByIds(route.StopIds, cancellationToken);
        var shopsById = shops.ToDictionary(shop => shop.Id);

        // A read answers in whatever order the rows come back, so the stops follow the ids.
        var stops = route.StopIds.Select((stopId, position) =>
        {
            if (!shopsById.TryGetValue(stopId.ToString(), out var shop))
            {
                _logger.LogWarning("Route stop {ShopId} names no shop", stopId);

                throw new NotFoundException(EntityTypes.Shop, stopId);
            }

            return new RouteStopModel
            {
                Id = shop.Id,
                Name = shop.Name,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                PlaceStatus = shop.PlaceStatus,
                Position = position,
            };
        }).ToArray();

        var stopCoordinates = stops
            .Select(stop => new CoordinateModel { Latitude = stop.Latitude, Longitude = stop.Longitude })
            .ToArray();

        IReadOnlyCollection<CoordinateModel> waypoints = route.Mode switch
        {
            RouteModes.Loop => [..stopCoordinates, stopCoordinates[0]],
            _ when route.Origin is not null => [route.Origin, ..stopCoordinates],
            _ => stopCoordinates
        };

        var geometry = await _googleRoutesClient.ComputeWalkingRoute(waypoints, cancellationToken);
        if (geometry is null)
        {
            _logger.LogWarning("Google Routes found no walking route through {WaypointCount} waypoints", waypoints.Count);

            throw new InvalidRequestException(EntityTypes.Route);
        }

        // Each leg is carried by the stop it arrives at: with an origin leg i arrives at stop i, without
        // one the walk starts at stop 0, and a loop's last leg lands back where it began.
        var legs = geometry.Legs.ToArray();
        var arrivingLegs = new RouteLegModel?[stops.Length];
        var arrivalOffset = route.Origin is null ? 1 : 0;

        for (var legIndex = 0; legIndex < legs.Length; legIndex++)
        {
            var arrivingStop = route.Mode == RouteModes.Loop
                ? (legIndex + 1) % stops.Length
                : legIndex + arrivalOffset;

            if (arrivingStop < stops.Length)
            {
                arrivingLegs[arrivingStop] = legs[legIndex];
            }
        }

        var walkingMinutes = (int)Math.Round(geometry.Walk.DurationSeconds / SecondsPerMinute);

        return new RouteShapeModel
        {
            Stops = stops.Select((stop, position) => stop with { Leg = arrivingLegs[position] }).ToArray(),
            Polyline = geometry.Walk.Polyline,
            DistanceMeters = geometry.Walk.DistanceMeters,
            TotalMinutes = walkingMinutes + stops.Length * BrowseMinutesPerStop,
            Mode = route.Mode,
        };
    }
}
