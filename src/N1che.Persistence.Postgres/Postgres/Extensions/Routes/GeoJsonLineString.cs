using System.Text.Json;
using N1che.Domain.Models.Routes;

namespace N1che.Persistence.Postgres.Postgres.Extensions.Routes;

/// <summary>The GeoJSON LineStrings PostGIS reads and writes a geography as.</summary>
internal static class GeoJsonLineString
{
    private const string LineStringType = "LineString";
    private const string CoordinatesProperty = "coordinates";
    private const int LongitudeIndex = 0;
    private const int LatitudeIndex = 1;

    // GeoJSON positions are [longitude, latitude], the reverse of how the app carries them.
    internal static string FromCoordinates(IReadOnlyCollection<CoordinateModel> coordinates) =>
        JsonSerializer.Serialize(new
        {
            type = LineStringType,
            coordinates = coordinates.Select(point => new[] { point.Longitude, point.Latitude }).ToArray()
        });

    internal static IReadOnlyCollection<CoordinateModel> ToCoordinates(string lineString)
    {
        using var document = JsonDocument.Parse(lineString);

        return document.RootElement.GetProperty(CoordinatesProperty)
            .EnumerateArray()
            .Select(position => new CoordinateModel
            {
                Longitude = position[LongitudeIndex].GetDouble(),
                Latitude = position[LatitudeIndex].GetDouble(),
            })
            .ToArray();
    }
}
