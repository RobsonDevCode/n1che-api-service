using N1che.Contracts.Response.Routes;

namespace N1che.ServiceTests.Infrastructure.Google;

// One turn Google is standing by to return inside a leg.
internal sealed record GoogleStep
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public string? Instruction { get; init; }

    public string? Maneuver { get; init; }
}
