namespace N1che.Domain.Models.Routes;

public record RoutesFilterModel
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required double RadiusMeters { get; init; }

    public required string Niche { get; init; }

    public required int Limit { get; init; }
}
