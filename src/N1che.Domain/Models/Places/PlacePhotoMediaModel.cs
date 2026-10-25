namespace N1che.Domain.Models.Places;

/// <summary>Where a place photo's image can be loaded from.</summary>
public record PlacePhotoMediaModel
{
    /// <summary>A short-lived public address of the image, carrying no API key.</summary>
    public required string PhotoUri { get; init; }
}
