namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>What Google's photo media endpoint returns when asked not to redirect.</summary>
public sealed record PlacePhotoMediaResponse
{
    /// <summary>A short-lived public address of the image, carrying no API key.</summary>
    public string? PhotoUri { get; init; }
}
