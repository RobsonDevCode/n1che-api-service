using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Persistence.Writers.Shops;

/// <summary>Writes shop trading hours to the store.</summary>
public interface IShopHoursWriter
{
    /// <summary>Creates the shop's trading hours, one row per day; writes nothing when there are none.</summary>
    Task Create(Guid shopId, IReadOnlyCollection<ShopHoursModel> hours, CancellationToken cancellationToken);
}
