using N1che.Domain.Models.Users;

namespace N1che.Domain.Interfaces.Services.Shops;

/// <summary>Records and withdraws a user's bookmark of a shop.</summary>
public interface IShopBookmarkingService
{
    /// <summary>
    /// Records the user's bookmark of the shop. Throws
    /// <see cref="Exceptions.NotFoundException"/> when no shop has that id, and
    /// <see cref="Exceptions.DuplicateRequestException"/> when the user has already bookmarked it.
    /// </summary>
    Task AddAsync(Guid shopId, UserModel user, CancellationToken cancellationToken);

    /// <summary>
    /// Withdraws the user's bookmark of the shop. Throws
    /// <see cref="Exceptions.NotFoundException"/> when no shop has that id, or when the user has not
    /// bookmarked it.
    /// </summary>
    Task RemoveAsync(Guid shopId, UserModel user, CancellationToken cancellationToken);
}
