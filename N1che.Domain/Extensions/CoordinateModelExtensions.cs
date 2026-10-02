using N1che.Domain.Models.Routes;

namespace N1che.Domain.Extensions;

public static class CoordinateModelExtensions
{
    private const double EarthRadiusMeters = 6_371_000;

    /// <summary>The great-circle (haversine) distance between two points.</summary>
    public static double DistanceMetersTo(this CoordinateModel from, CoordinateModel to)
    {
        var latitudeDelta = DegreesToRadians(to.Latitude - from.Latitude);
        var longitudeDelta = DegreesToRadians(to.Longitude - from.Longitude);

        var haversine = Math.Sin(latitudeDelta / 2) * Math.Sin(latitudeDelta / 2)
                        + Math.Cos(DegreesToRadians(from.Latitude)) * Math.Cos(DegreesToRadians(to.Latitude))
                        * Math.Sin(longitudeDelta / 2) * Math.Sin(longitudeDelta / 2);

        return 2 * EarthRadiusMeters * Math.Asin(Math.Sqrt(haversine));
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
}
