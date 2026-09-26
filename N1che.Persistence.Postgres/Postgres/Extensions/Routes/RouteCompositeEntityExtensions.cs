using System.Text.Json;
using N1che.Domain.Models.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

public static class RouteCompositeEntityExtensions
{
    private const string GeoJsonCoordinatesProperty = "coordinates";
    private const int LongitudeIndex = 0;
    private const int LatitudeIndex = 1;

    public static RouteModel ToDomainModel(this RouteCompositeEntity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Tag = entity.Tag,
        Mode = entity.Mode,
        Niche = entity.Niche,
        CreatedByUserId = entity.CreatedByUserId,
        CreatedByUsername = entity.CreatedByUsername,
        Stops = StopsFromJson(entity.StopsJson),
        Polyline = CoordinatesFromGeoJson(entity.PolylineGeoJson),
        DistanceMeters = entity.DistanceMeters,
        TotalMinutes = entity.TotalMinutes,
        VoteCount = entity.VoteCount,
        CreatedAt = entity.CreatedAt,
    };

    private static IReadOnlyCollection<RouteStopModel> StopsFromJson(string stops) =>
        JsonSerializer.Deserialize<IReadOnlyCollection<RouteStopCompositeEntity>>(stops, JsonSerializerOptions.Web)
            ?.Select(stop => stop.ToDomainModel())
            .ToArray() ?? [];

    // GeoJSON positions are [longitude, latitude], the reverse of how the app carries them.
    private static IReadOnlyCollection<CoordinateModel> CoordinatesFromGeoJson(string lineString)
    {
        using var document = JsonDocument.Parse(lineString);

        return document.RootElement.GetProperty(GeoJsonCoordinatesProperty)
            .EnumerateArray()
            .Select(position => new CoordinateModel
            {
                Longitude = position[LongitudeIndex].GetDouble(),
                Latitude = position[LatitudeIndex].GetDouble(),
            })
            .ToArray();
    }
}
