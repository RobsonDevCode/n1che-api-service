using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Readers.Shops;

/// <summary>Reads a user's interactions with a shop from the store.</summary>
public interface IShopInteractionsReader
{
    /// <summary>
    /// Gets the shop's vote count with the given user's interaction state for it,
    /// or <c>null</c> when no shop has that id.
    /// </summary>
    Task<ShopInteractionsModel?> GetByShopId(Guid shopId, string userId, CancellationToken cancellationToken);
}
