namespace N1che.Domain.Models.Routes;

public record RouteStopModel
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required string PlaceStatus { get; init; }

    public required int Position { get; init; }

    /// <summary>The walk that arrives at this stop, absent on a route read back from the store.</summary>
    public RouteLegModel? Leg { get; init; }
}
