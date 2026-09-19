namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>What a Google Places text search returns; Google omits the field when nothing matched.</summary>
public sealed record PlaceSearchResponse
{
    public IReadOnlyCollection<PlaceSummaryResponse>? Places { get; init; }
}
