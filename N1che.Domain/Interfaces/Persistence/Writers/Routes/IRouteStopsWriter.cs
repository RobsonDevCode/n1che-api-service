namespace N1che.Domain.Interfaces.Persistence.Writers.Routes;

/// <summary>Writes the stops a route walks through to the store.</summary>
public interface IRouteStopsWriter
{
    /// <summary>Creates the route's stops at the position the ids give them.</summary>
    Task Create(Guid routeId, IReadOnlyDictionary<Guid, int> stopPositions, CancellationToken cancellationToken);
}
