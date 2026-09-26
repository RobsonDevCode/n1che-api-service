using System.Globalization;
using N1che.Domain.Models.Routes;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class ComputeRoutesResponseExtensions
{
    private const string UnknownManeuver = "unknown";

    // Google carries a duration as protobuf seconds, e.g. "620s", and a fractional one separates on a
    // point whatever the host's culture does.
    private const char SecondsSuffix = 's';

    /// <summary>
    /// Reads the computed walk, or <c>null</c> when Google routed nothing. Alternatives are switched
    /// off, so the first route is the only one.
    /// </summary>
    public static RouteGeometryModel? ToDomainModel(this ComputeRoutesResponse response)
    {
        var route = response.Routes?.FirstOrDefault();

        return route is null
            ? null
            : new RouteGeometryModel
            {
                Walk = new RouteWalkModel
                {
                    DistanceMeters = route.DistanceMeters ?? 0,
                    DurationSeconds = ToSeconds(route.Duration),
                    Polyline = route.Polyline.ToDomainModels(),
                },
                Legs = (route.Legs ?? []).Select(leg => leg.ToDomainModel()).ToArray(),
            };
    }

    private static RouteLegModel ToDomainModel(this RouteLegResponse leg) => new()
    {
        Walk = new RouteWalkModel
        {
            DistanceMeters = leg.DistanceMeters ?? 0,
            DurationSeconds = ToSeconds(leg.Duration),
            Polyline = leg.Polyline.ToDomainModels(),
        },
        Steps = (leg.Steps ?? []).Select(step => step.ToDomainModel()).ToArray(),
    };

    private static RouteStepModel ToDomainModel(this RouteStepResponse step) => new()
    {
        Walk = new RouteWalkModel
        {
            DistanceMeters = step.DistanceMeters ?? 0,
            DurationSeconds = ToSeconds(step.StaticDuration),
            Polyline = step.Polyline.ToDomainModels(),
        },
        Instruction = step.NavigationInstruction?.Instructions ?? string.Empty,
        Maneuver = step.NavigationInstruction?.Maneuver ?? UnknownManeuver,
    };

    // GeoJSON orders a coordinate longitude first; a pair holding less than both is not a point.
    private static IReadOnlyCollection<CoordinateModel> ToDomainModels(this RoutePolylineResponse? polyline) =>
        (polyline?.GeoJsonLinestring?.Coordinates ?? [])
        .Where(pair => pair.Count >= 2)
        .Select(pair => new CoordinateModel
        {
            Longitude = pair.ElementAt(0),
            Latitude = pair.ElementAt(1),
        })
        .ToArray();

    private static int ToSeconds(string? duration) =>
        duration is null
            ? 0
            : (int)Math.Round(double.TryParse(
                duration.TrimEnd(SecondsSuffix),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var seconds)
                ? seconds
                : 0);
}
