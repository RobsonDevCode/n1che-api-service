using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.ThirdParty;

/// <summary>Computes walking geometry with Google Routes.</summary>
public interface IGoogleRoutesClient
{
    /// <summary>
    /// Walks the waypoints in the order given, first to last, and returns the geometry with one leg per
    /// consecutive pair; <c>null</c> when Google can find no walking route between them. Ordering is the
    /// caller's: the waypoints arrive as the route is walked, origin and loop closure included. Throws
    /// <see cref="Exceptions.GoogleRoutesException"/> when Google itself fails to answer.
    /// </summary>
    Task<RouteGeometryModel?> ComputeWalkingRoute(
        IReadOnlyCollection<CoordinateModel> waypoints, CancellationToken cancellationToken);
}
