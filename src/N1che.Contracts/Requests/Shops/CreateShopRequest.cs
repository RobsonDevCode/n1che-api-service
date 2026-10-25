namespace N1che.Contracts.Requests.Shops;

/// <summary>
/// A Google place being added as a shop. The submitting user is taken from the access token, never
/// the body; the place's name, address, location and trading hours are resolved server-side from
/// Google Places, so a caller cannot file a place under details of its own.
/// </summary>
public record CreateShopRequest
{
    /// <summary>Google Places identifier of the place being added.</summary>
    public required string GooglePlaceId { get; init; }

    /// <summary>Niches the shop belongs to; each must be a niche the niches endpoint returns.</summary>
    public required IReadOnlyCollection<string> Niches { get; init; }
}
