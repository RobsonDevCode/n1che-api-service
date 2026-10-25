using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Shops;

public static class ShopEntityExtensions
{
    public static ShopModel ToDomainModel(this ShopEntity entity) => new()
    {
        Id = entity.Id,
        GooglePlaceId = entity.GooglePlaceId,
        Name = entity.Name,
        Niches = entity.Niches,
        Address = entity.Address,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        VoteCount = entity.VoteCount,
        PlaceStatus = entity.PlaceStatus,
        CreatedAt = entity.CreatedAt,
        AddedByUserId = entity.AddedByUserId,
        AddedByUsername = entity.AddedByUsername,
    };
}
