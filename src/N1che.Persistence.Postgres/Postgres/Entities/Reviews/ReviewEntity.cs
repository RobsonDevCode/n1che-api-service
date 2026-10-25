namespace N1che.Persistence.Postgres.Postgres.Entities.Reviews;

public sealed record ReviewEntity
{
    public required Guid Id { get; init; }

    public required Guid ShopId { get; init; }

    public required string Niche { get; init; }

    public required string UserId { get; init; }

    public required string Username { get; init; }

    public required string Body { get; init; }

    public required int UpdateCount { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}
