using System.Text.Json;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class RouteDetailCompositeEntityExtensions
{
    public static RouteDetailModel ToDomainModel(this RouteDetailCompositeEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Tag = entity.Tag,
        Niche = entity.Niche,
        CreatedByUserId = entity.CreatedByUserId,
        CreatedByUsername = entity.CreatedByUsername,
        Stops = JsonSerializer.Deserialize<IReadOnlyCollection<RouteStopCompositeEntity>>(entity.StopsJson, JsonSerializerOptions.Web)
            ?.Select(stop => stop.ToDomainModel())
            .ToArray() ?? [],
        CreatedAt = entity.CreatedAt,
    };
}
