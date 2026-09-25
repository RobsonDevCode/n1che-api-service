namespace N1che.Domain.Interfaces.Persistence.Writers.Shops;

/// <summary>Writes a user's votes for shops to the store.</summary>
public interface IShopVotesWriter
{
    /// <summary>
    /// Records the user's vote for the shop. Throws
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already voted for it, and
    /// <see cref="Exceptions.NotFoundException"/> when no shop has that id.
    /// </summary>
    Task Create(Guid shopId, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes the user's vote for the shop, returning whether one was there to remove.
    /// </summary>
    Task<bool> Delete(Guid shopId, string userId, CancellationToken cancellationToken);
}
