namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

public sealed record RouteEntity
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Mode { get; init; }

    public required string Niche { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }

    public required double AnchorLatitude { get; init; }

    public required double AnchorLongitude { get; init; }

    /// <summary>The stored LineString carried as a GeoJSON document.</summary>
    public required string PolylineGeoJson { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    public required int VoteCount { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}
