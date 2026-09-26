namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>
/// A walking route through an ordered set of waypoints. The stops between the ends are
/// <c>intermediates</c>, which Google visits in the order they are given rather than optimising.
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
