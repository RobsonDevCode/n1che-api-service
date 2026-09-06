using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Readers.Shops;

public interface IShopsReader
{
    Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken);

    Task<PaginationModel<ShopModel>> GetPage(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken);
}
