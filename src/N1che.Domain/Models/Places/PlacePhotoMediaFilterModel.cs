namespace N1che.Domain.Models.Places;

/// <summary>The place photo whose image to fetch and the width to fetch it at.</summary>
public record PlacePhotoMediaFilterModel
{
    public required string Reference { get; init; }

    public required int MaxWidth { get; init; }
}
