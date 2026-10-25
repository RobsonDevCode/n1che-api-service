using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

/// <summary>Retrieves the calling user's interaction state for a route.</summary>
public interface IRouteInteractionsRetrievalService
{
    /// <summary>
    /// Gets the route's vote count with the given user's interaction state for it. Always read
    /// live, never cached, since the result is per-user.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no route has that id.
    /// </summary>
    Task<RouteInteractionsModel> GetByRouteIdAsync(Guid routeId, string userId, CancellationToken cancellationToken);
}
