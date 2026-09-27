using System.Text.Json;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class RouteCompositeEntityExtensions
{
    public static RouteModel ToDomainModel(this RouteCompositeEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Tag = entity.Tag,
        Niche = entity.Niche,
        CreatedByUserId = entity.CreatedByUserId,
        CreatedByUsername = entity.CreatedByUsername,
        Stops = StopsFromJson(entity.StopsJson),
        Polyline = GeoJsonLineString.ToCoordinates(entity.PolylineGeoJson),
        DistanceMeters = entity.DistanceMeters,
        TotalMinutes = entity.TotalMinutes,
        VoteCount = entity.VoteCount,
        CreatedAt = entity.CreatedAt,
    };

    private static IReadOnlyCollection<RouteStopModel> StopsFromJson(string stops) =>
        JsonSerializer.Deserialize<IReadOnlyCollection<RouteStopCompositeEntity>>(stops, JsonSerializerOptions.Web)
            ?.Select(stop => stop.ToDomainModel())
            .ToArray() ?? [];
}
