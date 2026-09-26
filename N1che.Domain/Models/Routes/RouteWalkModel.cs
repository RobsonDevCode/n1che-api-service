namespace N1che.Domain.Models.Routes;

/// <summary>
/// A stretch of walking: how far it runs, how long it takes, and the line it follows. A whole route's
/// geometry, one of its legs and one of a leg's steps are each a stretch of walking with something
/// added, so they all extend this.
/// </summary>
public record RouteWalkModel
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }
}
