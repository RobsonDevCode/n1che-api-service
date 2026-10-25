namespace N1che.Domain.Models.Shops;

public record NearbyShopsFilter
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required double RadiusMeters { get; init; }

    public string? Niche { get; init; }

    public required int Limit { get; init; }
}
