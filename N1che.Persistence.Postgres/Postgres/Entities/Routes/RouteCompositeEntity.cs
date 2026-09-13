namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>A route joined to the stops it holds, aggregated in position order.</summary>
public sealed record RouteCompositeEntity
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Mode { get; init; }

    public required string Niche { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }

    /// <summary>The stored LineString read back as a GeoJSON document.</summary>
    public required string PolylineGeoJson { get; init; }

    /// <summary>The route's stops, joined to their live shop rows and aggregated in position order.</summary>
    public required string StopsJson { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    public required int VoteCount { get; init; }

    public required DateTime CreatedAt { get; init; }
}
