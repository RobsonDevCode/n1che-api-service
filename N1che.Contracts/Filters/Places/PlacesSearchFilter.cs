namespace N1che.Contracts.Filters.Places;

/// <summary>
/// Filters a Google Places search for a place to add as a shop. The caller supplies the area to search
/// as the corners of a box — the map's visible region — and Google returns nothing outside it, so how
/// wide the search casts is the client's to decide by zoom rather than a radius fixed by the API.
/// </summary>
public record PlacesSearchFilter
{
    /// <summary>Free text to search for; required, since Google has no query-less text search.</summary>
    public required string Query { get; init; }

    /// <summary>Latitude of the south-west corner of the area to search, in decimal degrees (WGS 84).</summary>
    public required double SwLat { get; init; }

    /// <summary>Longitude of the south-west corner of the area to search, in decimal degrees (WGS 84).</summary>
    public required double SwLng { get; init; }

    /// <summary>Latitude of the north-east corner of the area to search, in decimal degrees (WGS 84).</summary>
    public required double NeLat { get; init; }

    /// <summary>
    /// Longitude of the north-east corner of the area to search, in decimal degrees (WGS 84). May be
    /// less than <see cref="SwLng"/>, which means the area crosses the antimeridian.
    /// </summary>
    public required double NeLng { get; init; }
}
