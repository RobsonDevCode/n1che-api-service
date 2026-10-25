namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>A Google Places text search, restricted to the area the caller is searching in.</summary>
public sealed record PlaceSearchRequest
{
    public required string TextQuery { get; init; }

    /// <summary>Results per page; Google caps this at 20 and ignores the deprecated maxResultCount.</summary>
    public required int PageSize { get; init; }

    public required PlaceLocationRestrictionRequest LocationRestriction { get; init; }
}
