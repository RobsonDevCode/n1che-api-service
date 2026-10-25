namespace N1che.Domain.Models.Reviews;

public record CreateReviewRequestModel
{
    public required Guid ShopId { get; init; }

    public required string Niche { get; init; }

    public required string Text { get; init; }

    public required string UserId { get; init; }

    public required string Username { get; init; }
}
