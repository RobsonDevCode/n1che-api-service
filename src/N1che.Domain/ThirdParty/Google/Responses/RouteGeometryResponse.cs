namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>One route Google computed, whole, with its legs.</summary>
public sealed record RouteGeometryResponse
{
    public double? DistanceMeters { get; init; }

    public string? Duration { get; init; }

    public RoutePolylineResponse? Polyline { get; init; }

    public IReadOnlyCollection<RouteLegResponse>? Legs { get; init; }
}
