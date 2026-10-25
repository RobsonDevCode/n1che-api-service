using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Services.Shops;

/// <summary>Retrieves the calling user's interaction state for a shop.</summary>
public interface IShopInteractionsRetrievalService
{
    /// <summary>
    /// Gets the shop's vote count with the given user's interaction state for it. Always read
    /// live, never cached, since the result is per-user.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no shop has that id.
    /// </summary>
    Task<ShopInteractionsModel> GetByShopIdAsync(Guid shopId, string userId, CancellationToken cancellationToken);
}
