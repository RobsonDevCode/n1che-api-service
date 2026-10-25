namespace N1che.Contracts.Response.Reviews;

public record ReviewResponse
{
    public required Guid Id { get; init; }

    public required Guid ShopId { get; init; }

    public required string Niche { get; init; }

    public required string UserId { get; init; }

    public required string Username { get; init; }

    public required string Text { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}
