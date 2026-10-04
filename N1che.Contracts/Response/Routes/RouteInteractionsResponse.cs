namespace N1che.Contracts.Response.Routes;

/// <summary>
/// The calling user's own state for a route, alongside the route's live vote count. Per-viewer and
/// volatile, so it is served separately from the cacheable route detail and never cached.
/// </summary>
public record RouteInteractionsResponse
{
    /// <summary>Total upvotes the route has received, across every user.</summary>
    public required int VoteCount { get; init; }

    /// <summary>Whether the calling user has upvoted the route.</summary>
    public required bool Voted { get; init; }
}
