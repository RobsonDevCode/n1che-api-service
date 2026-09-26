using N1che.Contracts.Response.Routes;

namespace N1che.ServiceTests.Infrastructure.Google;

// The walk between two waypoints Google is standing by to return. Legs come back in the order the
// waypoints were sent, which is what decides the stop each one is carried by.
internal sealed record GoogleLeg
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public IReadOnlyCollection<GoogleStep> Steps { get; init; } = [];
}
