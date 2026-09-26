namespace N1che.Contracts.Response.Routes;

/// <summary>
/// A computed route: nothing is saved, so it carries no id, author or vote count, and the caller keeps
/// the name and tag it asked with.
/// </summary>
public record RouteShapeResponse
{
    public required IReadOnlyCollection<RouteStopResponse> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    /// <summary><c>you</c> or <c>loop</c>, as the route was asked for.</summary>
    public required string Mode { get; init; }
}
