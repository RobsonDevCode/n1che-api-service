using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Readers.Shops;

/// <summary>Reads shops from the store.</summary>
public interface IShopsReader
{
    /// <summary>Gets the shops within the filter's radius, nearest first.</summary>
    Task<IReadOnlyCollection<ShopModel>> GetNearby(NearbyShopsFilter filter, CancellationToken cancellationToken);

    /// <summary>Gets a page of shops ordered most-upvoted first.</summary>
    Task<PaginationModel<ShopModel>> GetPage(ShopsFilterModel filterModel, PaginationDetailsModel pagination, CancellationToken cancellationToken);

    /// <summary>Gets a single shop by its identifier, or <c>null</c> when no shop has that id.</summary>
    Task<ShopModel?> GetById(Guid id, CancellationToken cancellationToken);

    /// <summary>Gets whether a shop has that id, without reading the row.</summary>
    Task<bool> Exists(Guid id, CancellationToken cancellationToken);
}
