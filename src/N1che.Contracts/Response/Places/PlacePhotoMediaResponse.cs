namespace N1che.Contracts.Response.Places;

/// <summary>Where a place photo's image can be loaded from.</summary>
public record PlacePhotoMediaResponse
{
    /// <summary>
    /// A short-lived public address of the image, needing no token. Load it straight away and do not
    /// store it: it expires, and Google's terms do not allow it to be cached.
    /// </summary>
    public required string PhotoUri { get; init; }
}
