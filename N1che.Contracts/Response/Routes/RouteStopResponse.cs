namespace N1che.Contracts.Response.Routes;

public record RouteStopResponse
{
    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required string PlaceStatus { get; init; }

    /// <summary>The walk that arrives here, absent on a saved route.</summary>
    public RouteLegResponse? Leg { get; init; }
}
