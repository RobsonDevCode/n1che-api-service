namespace N1che.Contracts.Response.Routes;

/// <summary>The walk that arrives at a stop, with its turn-by-turn steps.</summary>
public record RouteLegResponse
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public required IReadOnlyCollection<RouteStepResponse> Steps { get; init; }
}
