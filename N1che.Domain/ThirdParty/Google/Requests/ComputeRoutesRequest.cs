namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>
/// A walking route through an ordered set of waypoints, where the points between the ends are
/// <c>intermediates</c>.
/// </summary>
public sealed record ComputeRoutesRequest
{
    public required RouteWaypointRequest Origin { get; init; }

    public required RouteWaypointRequest Destination { get; init; }

    public required IReadOnlyCollection<RouteWaypointRequest> Intermediates { get; init; }

    public required string TravelMode { get; init; }

    public required string PolylineEncoding { get; init; }

    public required bool ComputeAlternativeRoutes { get; init; }

    public required string LanguageCode { get; init; }

    public required string Units { get; init; }
}
