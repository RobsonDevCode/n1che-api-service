using N1che.Domain.Models.Users;

namespace N1che.Domain.Interfaces.Services.Shops;

/// <summary>Records and withdraws a user's vote for a shop.</summary>
public interface IShopVotingService
{
    /// <summary>
    /// Records the user's vote and moves the shop's vote counter with it, in one transaction.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no shop has that id, and
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already voted for it.
    /// </summary>
    Task AddAsync(Guid shopId, UserModel user, CancellationToken cancellationToken);

    /// <summary>
    /// Withdraws the user's vote and moves the shop's vote counter with it, in one transaction.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no shop has that id, or when the user
    /// has not voted for it.
    /// </summary>
    Task RemoveAsync(Guid shopId, UserModel user, CancellationToken cancellationToken);
}
