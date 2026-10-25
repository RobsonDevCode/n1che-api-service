namespace N1che.Contracts.Filters.Places;

/// <summary>
/// The place photo whose image to fetch and the width to fetch it at. The place and photo ids are the
/// two ids in a photo reference, <c>places/{placeId}/photos/{photoId}</c>.
/// </summary>
public record PlacePhotoMediaFilter
{
    /// <summary>Google Places identifier of the place the photo is of.</summary>
    public required string PlaceId { get; init; }

    /// <summary>Google's identifier for the photo within the place.</summary>
    public required string PhotoId { get; init; }

    /// <summary>Widest the image may be, in pixels, from 1 to 4800; Google keeps the aspect ratio.</summary>
    public required int MaxWidth { get; init; }
}
