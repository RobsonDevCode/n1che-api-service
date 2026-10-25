namespace N1che.Domain.Models.Reviews;

public record UpdateReviewRequestModel
{
    public required Guid Id { get; init; }

    public required Guid ShopId { get; init; }

    public required string Text { get; init; }

    public required string UserId { get; init; }
}
