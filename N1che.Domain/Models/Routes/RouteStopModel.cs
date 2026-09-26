namespace N1che.Domain.Models.Routes;

public record RouteStopModel
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required string PlaceStatus { get; init; }

    public required int Position { get; init; }

    /// <summary>
    /// The walk that arrives at this stop. Only a route computed against Google carries legs; a saved
    /// route read back from the store leaves them absent.
    /// </summary>
    public RouteLegModel? Leg { get; init; }
}
