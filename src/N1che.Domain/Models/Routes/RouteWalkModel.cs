namespace N1che.Domain.Models.Routes;

/// <summary>A stretch of walking: how far it runs, how long it takes, and the line it follows.</summary>
public record RouteWalkModel
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }
}
