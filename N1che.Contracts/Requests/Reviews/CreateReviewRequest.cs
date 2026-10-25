namespace N1che.Contracts.Requests.Reviews;

/// <summary>A review of a shop. The author is taken from the access token, never the body.</summary>
public record CreateReviewRequest
{
    public required string Text { get; init; }

    /// <summary>The niche the review is written in; must be one of the shop's niches.</summary>
    public required string Niche { get; init; }
}
