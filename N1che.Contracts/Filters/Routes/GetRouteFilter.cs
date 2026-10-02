namespace N1che.Contracts.Filters.Routes;

/// <summary>Where the walk through a saved route begins.</summary>
public record GetRouteFilter
{
    public required double Lat { get; init; }

    public required double Lng { get; init; }
}
