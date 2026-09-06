using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Services.Shops;

public interface IShopRetrievalService
{
    Task<IReadOnlyCollection<ShopModel>> GetNearbyAsync(NearbyShopsFilter filter, CancellationToken cancellationToken);

    Task<PaginationModel<ShopModel>> GetPageAsync(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken);
}
