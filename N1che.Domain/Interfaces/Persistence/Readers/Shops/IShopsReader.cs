using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Readers.Shops;

public interface IShopsReader
{
    Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken);
}
