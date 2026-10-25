namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>The author Google credits for a photo, which must be shown wherever the photo is.</summary>
public sealed record PlaceAuthorAttributionResponse
{
    public string? DisplayName { get; init; }

    public string? Uri { get; init; }

    public string? PhotoUri { get; init; }
}
