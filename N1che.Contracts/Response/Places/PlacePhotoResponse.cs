namespace N1che.Contracts.Response.Places;

/// <summary>
/// A photo of a place. Google's terms require the photo to be shown with its author credit and a link
/// to its source on Google Maps; a thumbnail may leave the credit out only if tapping it opens a larger
/// version that shows the credit.
/// </summary>
public record PlacePhotoResponse
{
    /// <summary>Reference to the photo, to be exchanged for its image at the photo media endpoint.</summary>
    public required string Reference { get; init; }

    /// <summary>The photo's source on Google Maps, which users must always be able to open.</summary>
    public required string GoogleMapsUri { get; init; }

    /// <summary>The authors to credit wherever the photo is shown.</summary>
    public required IReadOnlyCollection<PhotoAuthorAttributionResponse> AuthorAttributions { get; init; }
}
