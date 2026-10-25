namespace N1che.Persistence.Postgres.Postgres.Entities.Shops;

/// <summary>One user's bookmark of one shop.</summary>
public sealed record BookmarkEntity
{
    public required Guid ShopId { get; init; }

    public required string UserId { get; init; }

    public required DateTime CreatedAt { get; init; }
}
