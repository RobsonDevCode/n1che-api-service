namespace N1che.Domain.Models.Routes;

/// <summary>
/// A walkable route: its stops in the order they are walked, the line through them, and what the walk
/// costs. This is the whole of a computed route — nothing is persisted, so it carries no identity,
/// author or vote count, which is what <see cref="RouteModel"/> adds for a saved one.
/// </summary>
public record RouteShapeModel
{
    public required IReadOnlyCollection<RouteStopModel> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    /// <summary>The walk plus a browse allowance per stop, as the app shows a route's time.</summary>
    public required int TotalMinutes { get; init; }

    /// <summary>The <see cref="RouteModes"/> the stops were ordered by.</summary>
    public required string Mode { get; init; }
}
