namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>
/// A place as returned by search, carrying only the requested field mask. Search returns a subset of
/// what Details holds, so every field is optional and a place missing one cannot be added as a shop.
/// </summary>
public sealed record PlaceSummaryResponse
{
    public string? Id { get; init; }

    public PlaceDisplayNameResponse? DisplayName { get; init; }

    public string? FormattedAddress { get; init; }

    public PlaceLocationResponse? Location { get; init; }

    public IReadOnlyCollection<PlacePhotoResponse>? Photos { get; init; }
}
