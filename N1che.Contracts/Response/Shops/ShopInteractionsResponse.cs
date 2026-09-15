namespace N1che.Contracts.Response.Shops;

/// <summary>
/// The calling user's own state for a shop, alongside the shop's live vote count. Per-viewer and
/// volatile, so it is served separately from the cacheable shop detail and never cached.
/// </summary>
public record ShopInteractionsResponse
{
    /// <summary>Total upvotes the shop has received, across every user.</summary>
    public required int VoteCount { get; init; }

    /// <summary>Whether the calling user has upvoted the shop.</summary>
    public required bool Voted { get; init; }

    /// <summary>Whether the calling user has bookmarked the shop.</summary>
    public required bool Saved { get; init; }
}
