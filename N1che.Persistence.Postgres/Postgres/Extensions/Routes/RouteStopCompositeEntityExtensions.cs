using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class RouteStopCompositeEntityExtensions
{
    public static RouteStopModel ToDomainModel(this RouteStopCompositeEntity entity) => new()
    {
        Id = entity.Id.ToString(),
        Name = entity.Name,
        Address = entity.Address,
        Latitude = entity.Latitude,
        Longitude = entity.Longitude,
        PlaceStatus = entity.PlaceStatus,
        Position = entity.Position,
    };
}
