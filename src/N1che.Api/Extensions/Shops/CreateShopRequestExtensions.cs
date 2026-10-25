using N1che.Contracts.Requests.Shops;
using N1che.Domain.Models.Shops;
using N1che.Domain.Models.Users;

namespace N1che.Api.Extensions.Shops;

public static class CreateShopRequestExtensions
{
    public static CreateShopRequestModel ToDomainModel(this CreateShopRequest request, UserModel addedBy) => new()
    {
        GooglePlaceId = request.GooglePlaceId,
        Niches = request.Niches,
        AddedByUserId = addedBy.Id,
        AddedByUsername = addedBy.Username,
    };
}
