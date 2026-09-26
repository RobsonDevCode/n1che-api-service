namespace N1che.Contracts.Response.Routes;

public record RouteResponse
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Mode { get; init; }

    public required string Niche { get; init; }

    public required string CreatedBy { get; init; }

    public required string UserId { get; init; }

    public required IReadOnlyCollection<RouteStopResponse> Stops { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    public required int TotalUpvotes { get; init; }

    public required DateTime CreatedAt { get; init; }
}
