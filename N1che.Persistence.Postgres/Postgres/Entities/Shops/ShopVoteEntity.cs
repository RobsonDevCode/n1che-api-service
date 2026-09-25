namespace N1che.Persistence.Postgres.Postgres.Entities.Shops;

/// <summary>One user's vote for one shop.</summary>
public sealed record ShopVoteEntity
{
    public required Guid ShopId { get; init; }

    public required string UserId { get; init; }

    public required DateTime CreatedAt { get; init; }
}
