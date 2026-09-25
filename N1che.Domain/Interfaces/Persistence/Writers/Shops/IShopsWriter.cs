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

    /// <summary>Adds one to the shop's denormalised vote counter.</summary>
    Task IncrementVoteCount(Guid shopId, CancellationToken cancellationToken);

    /// <summary>Takes one off the shop's denormalised vote counter, never below zero.</summary>
    Task DecrementVoteCount(Guid shopId, CancellationToken cancellationToken);
}
