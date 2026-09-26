using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

/// <summary>Computes the walk through a set of shops without saving anything.</summary>
public interface IRouteComputationService
{
    /// <summary>
    /// Computes the walk through the route's stops, in the order they were asked for and shaped by its
    /// mode. Throws <see cref="Exceptions.NotFoundException"/> when a stop names no shop we hold, and
    /// <see cref="Exceptions.InvalidRequestException"/> when no walking route runs through them.
    /// </summary>
    Task<RouteShapeModel> ComputeAsync(ComputeRouteModel route, CancellationToken cancellationToken);
}
