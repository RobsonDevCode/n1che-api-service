namespace N1che.Contracts.Requests.Reviews;

/// <summary>An edit to a review's text. Only the review's author can make it, and its niche never changes.</summary>
public record UpdateReviewRequest
{
    public required string Text { get; init; }
}
