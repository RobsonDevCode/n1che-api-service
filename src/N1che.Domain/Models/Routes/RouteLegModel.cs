namespace N1che.Domain.Models.Routes;

/// <summary>The walk between two consecutive points of a route, with its turn-by-turn steps.</summary>
public record RouteLegModel
{
    public required RouteWalkModel Walk { get; init; }

    public required IReadOnlyCollection<RouteStepModel> Steps { get; init; }
}
