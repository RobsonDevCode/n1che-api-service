namespace N1che.Domain.Models.Places;

/// <summary>A photo of a place, with what Google requires to be shown alongside it.</summary>
public record PlacePhotoModel
{
    /// <summary>Google's resource name for the photo, of the form <c>places/{placeId}/photos/{photoId}</c>.</summary>
    public required string Reference { get; init; }

    /// <summary>The photo's source on Google Maps, which users must always be able to open.</summary>
    public required string GoogleMapsUri { get; init; }

    public required IReadOnlyCollection<PhotoAuthorAttributionModel> AuthorAttributions { get; init; }
}
