namespace N1che.Contracts.Response.Places;

/// <summary>
/// A place Google holds, as returned by the places search. This is a candidate to add as a shop, not
/// a shop — it has no N1che identifier until it is posted to the shops endpoint.
/// </summary>
public record PlaceResponse
{
    /// <summary>Google Places identifier; the value to post to the shops endpoint to add this place.</summary>
    public required string GooglePlaceId { get; init; }

    /// <summary>Display name of the place.</summary>
    public required string Name { get; init; }

    /// <summary>Street address of the place.</summary>
    public required string Address { get; init; }

    /// <summary>Latitude in decimal degrees (WGS 84).</summary>
    public required double Latitude { get; init; }

    /// <summary>Longitude in decimal degrees (WGS 84).</summary>
    public required double Longitude { get; init; }

    /// <summary>
    /// Reference to the place's first photo, to be exchanged for the image at the photos endpoint.
    /// Absent when Google holds no photo of the place.
    /// </summary>
    public string? PhotoReference { get; init; }
}
