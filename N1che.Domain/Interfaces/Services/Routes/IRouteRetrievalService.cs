using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

public interface IRouteRetrievalService
{
    Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearbyAsync(RoutesFilterModel filterModel, CancellationToken cancellationToken);

    /// <summary>Gets a saved route as it was stored, stops and geometry included, without its vote count.</summary>
    /// <exception cref="Exceptions.NotFoundException">No route has the id.</exception>
    ValueTask<RouteDetailModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Gets a saved route with its walk computed from the origin through its stops, in order.</summary>
    /// <exception cref="Exceptions.NotFoundException">No route has the id.</exception>
    /// <exception cref="Exceptions.RouteException">The route's first stop is too far from the origin.</exception>
    /// <exception cref="Exceptions.InvalidRequestException">No walking route runs through the stops.</exception>
    Task<RouteDetailModel> GetComputedRouteAsync(Guid id, CoordinateModel origin, CancellationToken cancellationToken);
}
