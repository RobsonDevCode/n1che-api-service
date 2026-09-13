using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Services.Shops;

/// <summary>Retrieves shops for the shop endpoints.</summary>
public interface IShopRetrievalService
{
    /// <summary>Gets the shops within the filter's radius, nearest first.</summary>
    Task<IReadOnlyCollection<ShopModel>> GetNearbyAsync(NearbyShopsFilter filter, CancellationToken cancellationToken);

    /// <summary>Gets a page of shops ordered most-upvoted first.</summary>
    Task<PaginationModel<ShopModel>> GetPageAsync(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a single shop by its identifier, served from cache when it has been read recently.
    /// Throws <see cref="Exceptions.NotFoundException"/> when no shop has that id.
    /// </summary>
    ValueTask<ShopModel> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
