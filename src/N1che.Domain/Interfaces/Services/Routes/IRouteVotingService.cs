using N1che.Domain.Models.Users;

namespace N1che.Domain.Interfaces.Services.Routes;

/// <summary>Records and withdraws a user's vote for a route.</summary>
public interface IRouteVotingService
{
    /// <summary>
    /// Records the user's vote and moves the route's vote counter with it, in one transaction.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no route has that id, and
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already voted for it.
    /// </summary>
    Task AddAsync(Guid routeId, UserModel user, CancellationToken cancellationToken);

    /// <summary>
    /// Withdraws the user's vote and moves the route's vote counter with it, in one transaction.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no route has that id, or when the user
    /// has not voted for it.
    /// </summary>
    Task RemoveAsync(Guid routeId, UserModel user, CancellationToken cancellationToken);
}
