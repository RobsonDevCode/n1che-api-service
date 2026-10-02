namespace N1che.Domain.Models.Routes;

/// <summary>A single saved route without its vote count, which is served on its own so this stays cacheable.</summary>
public record RouteDetailModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Niche { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }

    public required IReadOnlyCollection<RouteStopModel> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    public required DateTime CreatedAt { get; init; }
}
