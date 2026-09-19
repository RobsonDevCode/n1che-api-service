using N1che.Domain.Models.Shops;

namespace N1che.Domain.Interfaces.Services.Shops;

/// <summary>Adds user-submitted shops.</summary>
public interface IShopCreationService
{
    /// <summary>
    /// Creates the shop from the place Google holds for the submitted identifier, with the trading
    /// hours Google holds for it, and returns it with today's hours. Throws
    /// <see cref="Exceptions.InvalidRequestException"/> when a niche is not recognised or the place is
    /// unknown to Google or permanently closed, <see cref="Exceptions.DuplicateRequestException"/>
    /// when the place has already been added, and <see cref="Exceptions.GooglePlacesException"/> when
    /// Google fails to answer.
    /// </summary>
    Task<ShopModel> CreateAsync(CreateShopRequestModel request, CancellationToken cancellationToken);
}
