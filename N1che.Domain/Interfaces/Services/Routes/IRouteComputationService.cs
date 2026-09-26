using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

/// <summary>Computes the walk through a set of shops.</summary>
public interface IRouteComputationService
{
    /// <summary>Computes the walk through the route's stops, in the order given and shaped by its mode.</summary>
    /// <exception cref="Exceptions.NotFoundException">A stop has no shop.</exception>
    /// <exception cref="Exceptions.InvalidRequestException">No walking route runs through the stops.</exception>
    Task<RouteShapeModel> ComputeAsync(ComputeRouteModel route, CancellationToken cancellationToken);
}
