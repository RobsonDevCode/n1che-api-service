using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Writers.Shops;

/// <summary>Writes shops to the store.</summary>
public interface IShopsWriter
{
    /// <summary>
    /// Creates the shop and returns it as stored. Throws
    /// <see cref="Exceptions.DuplicateRequestException"/> when the Google place has already been added.
    /// </summary>
    Task<ShopModel> Create(NewShopModel newShop, CancellationToken cancellationToken);
}
