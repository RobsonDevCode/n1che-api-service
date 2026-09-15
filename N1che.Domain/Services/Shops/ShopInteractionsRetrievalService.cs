using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Services.Shops;

public sealed class ShopInteractionsRetrievalService : IShopInteractionsRetrievalService
{
    private readonly IShopInteractionsReader _shopInteractionsReader;

    public ShopInteractionsRetrievalService(IShopInteractionsReader shopInteractionsReader)
    {
        _shopInteractionsReader = shopInteractionsReader;
    }

    public async Task<ShopInteractionsModel> GetByShopIdAsync(Guid shopId, string userId, CancellationToken cancellationToken)
    {
        return await _shopInteractionsReader.GetByShopId(shopId, userId, cancellationToken)
               ?? throw new NotFoundException(EntityTypes.Shop, shopId);
    }
}
