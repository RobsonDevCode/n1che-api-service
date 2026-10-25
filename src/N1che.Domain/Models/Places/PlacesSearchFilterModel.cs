namespace N1che.Domain.Models.Places;

/// <summary>The text a places search looks for and the box it is confined to.</summary>
public record PlacesSearchFilterModel
{
    public required string Query { get; init; }

    public required double SouthWestLatitude { get; init; }

    public required double SouthWestLongitude { get; init; }

    public required double NorthEastLatitude { get; init; }

    public required double NorthEastLongitude { get; init; }
}
