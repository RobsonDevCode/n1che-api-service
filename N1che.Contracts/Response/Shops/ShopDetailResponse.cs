namespace N1che.Contracts.Response.Shops;

/// <summary>
/// A single shop's static detail, as returned by the shop-by-id endpoint. Carries only fields that
/// change through ingestion or not at all — the mutable vote count and the shop's reviews are served
/// by the interactions and reviews endpoints so this payload stays cacheable.
/// </summary>
public record ShopDetailResponse
{
    /// <summary>Internal identifier for the shop.</summary>
    public required string Id { get; init; }

    /// <summary>Google Places identifier the shop was added from.</summary>
    public required string GooglePlaceId { get; init; }

    /// <summary>Display name of the shop.</summary>
    public required string Name { get; init; }

    /// <summary>Every niche the shop belongs to.</summary>
    public required IReadOnlyCollection<string> Niches { get; init; }

    /// <summary>Street address of the shop.</summary>
    public required string Address { get; init; }

    /// <summary>Latitude in decimal degrees (WGS 84).</summary>
    public required double Latitude { get; init; }

    /// <summary>Longitude in decimal degrees (WGS 84).</summary>
    public required double Longitude { get; init; }

    /// <summary>Whether the shop is still trading: <c>operational</c> or <c>closed</c>.</summary>
    public required string PlaceStatus { get; init; }

    /// <summary>Today's opening time, absent when the shop has no hours recorded for today.</summary>
    public TimeOnly? OpenTime { get; init; }

    /// <summary>Today's closing time, absent when the shop has no hours recorded for today or trades around the clock.</summary>
    public TimeOnly? CloseTime { get; init; }

    /// <summary>When the shop was added to N1che.</summary>
    public required DateTime CreatedAt { get; init; }

    /// <summary>Cognito subject of the user who added the shop.</summary>
    public required string AddedByUserId { get; init; }

    /// <summary>Username of the user who added the shop, snapshotted when it was added.</summary>
    public required string AddedByUsername { get; init; }

    /// <summary>Photo for the shop, absent when none has been resolved.</summary>
    public string? PhotoUrl { get; init; }
}
