using N1che.Domain.Models.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Shops;

public static class ShopCompositeEntityExtensions
{
    public static ShopModel ToDomainModel(this ShopCompositeEntity entity) => new()
    {
        Id = entity.Id.ToString(),
        GooglePlaceId = entity.GooglePlaceId,
        Name = entity.Name,
        Niches = entity.Niches,
        Address = entity.Address,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        VoteCount = entity.VoteCount,
        PlaceStatus = entity.PlaceStatus,
        OpenTime = entity.OpenTime,
        CloseTime = entity.CloseTime,
        CreatedAt = entity.CreatedAt,
        AddedByUserId = entity.AddedByUserId,
        AddedByUsername = entity.AddedByUsername,
    };
}
