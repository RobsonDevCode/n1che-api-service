namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>The identity the store gives a route as it is created.</summary>
public sealed record CreatedRouteEntity
{
    public required Guid Id { get; init; }

    public required DateTime CreatedAt { get; init; }
}
