namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>A single route joined to the stops it holds, without its geometry or vote count.</summary>
public sealed record RouteDetailCompositeEntity
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Niche { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }

    /// <summary>The route's stops, joined to their live shop rows and aggregated in position order.</summary>
    public required string StopsJson { get; init; }

    public required DateTime CreatedAt { get; init; }
}
