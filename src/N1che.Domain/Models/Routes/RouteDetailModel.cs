namespace N1che.Domain.Models.Routes;

/// <summary>
/// A single saved route's details and stops, without its stored geometry (replaced by a computed walk)
/// or its vote count (served on its own), so it stays cheap to cache.
/// </summary>
public record RouteDetailModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Niche { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }

    public required IReadOnlyCollection<RouteStopModel> Stops { get; init; }

    public required DateTime CreatedAt { get; init; }
}
