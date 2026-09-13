namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>A route stop joined to the live shop row it points at.</summary>
public sealed record RouteStopCompositeEntity
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required string PlaceStatus { get; init; }

    public required int Position { get; init; }
}
