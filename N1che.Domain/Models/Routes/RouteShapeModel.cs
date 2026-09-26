namespace N1che.Domain.Models.Routes;

/// <summary>
/// A walkable route: its stops in the order they are walked, the line through them, and what the walk
/// costs.
/// </summary>
public record RouteShapeModel
{
    public required IReadOnlyCollection<RouteStopModel> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    /// <summary>How long the walk takes.</summary>
    public required int TotalMinutes { get; init; }

    /// <summary>The <see cref="RouteModes"/> the stops were ordered by.</summary>
    public required string Mode { get; init; }
}
