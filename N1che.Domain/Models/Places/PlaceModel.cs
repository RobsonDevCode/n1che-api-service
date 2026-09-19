namespace N1che.Domain.Models.Places;

/// <summary>
/// A place a search turned up, holding only what the add-shop flow needs to show it and post it.
/// <see cref="PlaceDetailsModel"/> is the fuller shape read once a place is actually being added.
/// </summary>
public record PlaceModel
{
    public required string GooglePlaceId { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    /// <summary>Google's resource name for the place's first photo; null when it holds none.</summary>
    public string? PhotoReference { get; init; }
}
