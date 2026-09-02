using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Services.Shops;

public sealed class ShopRetrievalService : IShopRetrievalService
{
    private readonly IShopsReader _shopsReader;

    public ShopRetrievalService(IShopsReader shopsReader)
    {
        _shopsReader = shopsReader;
    }

    public Task<IReadOnlyCollection<ShopModel>> GetNearbyAsync(NearbyShopsFilter filter, CancellationToken cancellationToken)
    {
        return _shopsReader.GetNearby(filter, cancellationToken);
    }
}
