namespace N1che.Domain.Interfaces.Persistence.Writers.Shops;

/// <summary>Writes a user's bookmarked shops to the store.</summary>
public interface IBookmarksWriter
{
    /// <summary>
    /// Records the user's bookmark of the shop. Throws
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already bookmarked it, and
    /// <see cref="Exceptions.NotFoundException"/> when no shop has that id.
    /// </summary>
    Task Create(Guid shopId, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user's bookmark of the shop, returning whether one was there to remove.
    /// </summary>
    Task<bool> Delete(Guid shopId, string userId, CancellationToken cancellationToken);
}
