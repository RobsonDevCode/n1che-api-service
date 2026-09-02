namespace N1che.Contracts.Filters.Shops;

public record NearbyShopsFilter
{
    public required double Lat { get; init; }

    public required double Lng { get; init; }

    /// <summary>Search radius in metres.</summary>
    public double? Radius { get; init; }

    public string? Niche { get; init; }

    public int? Limit { get; init; }
}
