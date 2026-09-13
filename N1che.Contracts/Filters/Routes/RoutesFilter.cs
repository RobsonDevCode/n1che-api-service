namespace N1che.Contracts.Filters.Routes;

public record RoutesFilter
{
    public required double Lat { get; init; }

    public required double Lng { get; init; }

    /// <summary>Search radius in metres, measured from the route's anchor point.</summary>
    public double? Radius { get; init; }

    public required string Niche { get; init; }

    public int? Limit { get; init; }
}
