namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>A photo Google holds of a place.</summary>
public sealed record PlacePhotoResponse
{
    /// <summary>Google's resource name for the photo, of the form <c>places/{placeId}/photos/{photoId}</c>.</summary>
    public string? Name { get; init; }
}
