using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.ThirdParty;

/// <summary>Computes walking geometry with Google Routes.</summary>
public interface IGoogleRoutesClient
{
    /// <summary>
    /// Walks the waypoints in the order given, first to last, and returns the geometry with one leg per
    /// consecutive pair, or <c>null</c> when Google can find no walking route between them.
    /// </summary>
    /// <exception cref="Exceptions.GoogleRoutesException">Google failed to answer.</exception>
    Task<RouteGeometryModel?> ComputeWalkingRoute(
        IReadOnlyCollection<CoordinateModel> waypoints, CancellationToken cancellationToken);
}
