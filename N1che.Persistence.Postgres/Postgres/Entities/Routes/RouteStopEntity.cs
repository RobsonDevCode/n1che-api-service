namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>A shop's place in a route's walk.</summary>
public sealed record RouteStopEntity
{
    public required Guid RouteId { get; init; }

    public required Guid ShopId { get; init; }

    /// <summary>Zero-based, in the order the stops are walked.</summary>
    public required int Position { get; init; }
}
