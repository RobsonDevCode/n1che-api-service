namespace N1che.Domain.Models.Routes;

/// <summary>A saved route's details with its walk computed from an origin through its stops.</summary>
public record ComputedRouteModel
{
    public required RouteDetailModel Detail { get; init; }

    public required RouteShapeModel Walk { get; init; }
}
