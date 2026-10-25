using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Persistence.Readers.Routes;

/// <summary>Reads a user's interactions with a route from the store.</summary>
public interface IRouteInteractionsReader
{
    /// <summary>
    /// Gets the route's vote count, with the given user's interaction state for it when a user is
    /// given (otherwise <c>Voted</c> is <c>false</c>), or <c>null</c> when no route has that id.
    /// </summary>
    Task<RouteInteractionsModel?> GetById(Guid routeId, string? userId, CancellationToken cancellationToken);
}
