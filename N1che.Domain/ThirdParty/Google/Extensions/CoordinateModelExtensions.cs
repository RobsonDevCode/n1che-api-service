using N1che.Domain.Models.Routes;
using N1che.Domain.ThirdParty.Google.Requests;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class CoordinateModelExtensions
{
    private const string WalkingMode = "WALK";

    // GeoJSON hands back coordinates we can read straight off, where Google's own encoding would have
    // to be decoded first.
    private const string GeoJsonPolyline = "GEO_JSON_LINESTRING";

    // The app walks one route and draws one line, so alternatives would be billed for and dropped.
    private const bool AlternativeRoutes = false;

    private const string LanguageCode = "en-GB";
    private const string Units = "METRIC";

    /// <summary>
    /// Builds the request for a walk through the waypoints in the order given. The caller has ordered
    /// them, so the first and last are the ends of the walk and everything between is an intermediate,
    /// which Google visits in sequence rather than optimising.
    /// </summary>
    public static ComputeRoutesRequest ToGoogleRequest(this IReadOnlyCollection<CoordinateModel> waypoints) => new()
    {
        Origin = waypoints.First().ToGoogleWaypoint(),
        Destination = waypoints.Last().ToGoogleWaypoint(),
        Intermediates = waypoints.Skip(1).SkipLast(1).Select(waypoint => waypoint.ToGoogleWaypoint()).ToArray(),
        TravelMode = WalkingMode,
        PolylineEncoding = GeoJsonPolyline,
        ComputeAlternativeRoutes = AlternativeRoutes,
        LanguageCode = LanguageCode,
        Units = Units,
    };

    private static RouteWaypointRequest ToGoogleWaypoint(this CoordinateModel coordinate) => new()
    {
        Location = new RouteWaypointLocationRequest
        {
            LatLng = new PointRequest
            {
                Latitude = coordinate.Latitude,
                Longitude = coordinate.Longitude,
            }
        }
    };
}
