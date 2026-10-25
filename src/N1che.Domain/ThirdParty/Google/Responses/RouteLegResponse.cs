namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>The walk between two consecutive waypoints, with the steps that make it up.</summary>
public sealed record RouteLegResponse
{
    public double? DistanceMeters { get; init; }

    public string? Duration { get; init; }

    public RoutePolylineResponse? Polyline { get; init; }

    public IReadOnlyCollection<RouteStepResponse>? Steps { get; init; }
}
