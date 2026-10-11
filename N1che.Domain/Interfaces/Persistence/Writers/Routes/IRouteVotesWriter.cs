namespace N1che.Domain.Interfaces.Persistence.Writers.Routes;

/// <summary>Writes a user's votes for routes to the store.</summary>
public interface IRouteVotesWriter
{
    /// <summary>
    /// Records the user's vote for the route. Throws
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already voted for it, and
    /// <see cref="Exceptions.NotFoundException"/> when no route has that id.
    /// </summary>
    Task Create(Guid routeId, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user's vote for the route. Throws <see cref="Exceptions.NotFoundException"/> when
    /// the user has not voted for it.
    /// </summary>
    Task Delete(Guid routeId, string userId, CancellationToken cancellationToken);
}
