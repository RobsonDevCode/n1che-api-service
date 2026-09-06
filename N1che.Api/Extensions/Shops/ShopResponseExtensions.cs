using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Shops;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Shops;

namespace N1che.Api.Extensions.Shops;

public static class ShopResponseExtensions
{
    public static IReadOnlyCollection<ShopResponse> ToResponse(this IReadOnlyCollection<ShopModel> shops) =>
        shops.Select(shop => shop.ToResponse()).ToArray();

    public static PagedResponse<ShopResponse> ToPagedResponse(this PaginationModel<ShopModel> pagination) => new()
    {
        Data = pagination.Data.Select(shop => shop.ToResponse()).ToArray(),
        PaginationDetails = new PaginationDetailsResponse
        {
            TotalCount = pagination.TotalCount,
            CurrentPage = pagination.Page,
            PageSize = pagination.PageSize,
        }
    };

    public static ShopResponse ToResponse(this ShopModel shop) => new()
    {
        Id = shop.Id,
        GooglePlaceId = shop.GooglePlaceId,
        Name = shop.Name,
        Niches = shop.Niches,
        Address = shop.Address,
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
        VoteCount = shop.VoteCount,
        PlaceStatus = shop.PlaceStatus,
        CreatedAt = shop.CreatedAt,
        AddedByUserId = shop.AddedByUserId,
        AddedByUsername = shop.AddedByUsername,
        PhotoUrl = shop.PhotoUrl,
    };
}
