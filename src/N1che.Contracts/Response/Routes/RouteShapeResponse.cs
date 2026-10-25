namespace N1che.Contracts.Response.Routes;

/// <summary>
/// A walk through a set of stops: the stops in the order they are walked, the line through them, and
/// what the walk costs.
/// </summary>
public record RouteShapeResponse
{
    public required IReadOnlyCollection<RouteStopResponse> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    /// <summary><c>you</c> or <c>loop</c>.</summary>
    public required string Mode { get; init; }
}
