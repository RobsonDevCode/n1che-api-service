namespace N1che.Contracts.Response.Routes;

/// <summary>One turn-by-turn step of a leg.</summary>
public record RouteStepResponse
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public required string Instruction { get; init; }

    public required string Maneuver { get; init; }
}
