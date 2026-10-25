namespace N1che.Persistence.Postgres.Postgres.Entities.Shops;

/// <summary>A shop's vote counter joined to one user's interaction state for it.</summary>
public sealed record ShopInteractionsCompositeEntity
{
    public required int VoteCount { get; init; }

    public required bool Voted { get; init; }

    public required bool Saved { get; init; }
}
