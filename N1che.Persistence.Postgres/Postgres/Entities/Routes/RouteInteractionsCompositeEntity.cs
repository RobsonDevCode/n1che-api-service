namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>A route's vote counter joined to one user's interaction state for it.</summary>
public sealed record RouteInteractionsCompositeEntity
{
    public required int VoteCount { get; init; }

    public required bool Voted { get; init; }
}
