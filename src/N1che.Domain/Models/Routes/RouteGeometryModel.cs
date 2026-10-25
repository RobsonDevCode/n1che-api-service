namespace N1che.Domain.Models.Routes;

/// <summary>The walking geometry computed for an ordered set of waypoints.</summary>
public record RouteGeometryModel
{
    public required RouteWalkModel Walk { get; init; }

    /// <summary>One leg per consecutive pair of waypoints, in the order the waypoints were sent.</summary>
    public required IReadOnlyCollection<RouteLegModel> Legs { get; init; }
}
