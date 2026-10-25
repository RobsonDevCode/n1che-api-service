using N1che.Contracts.Response.Routes;

namespace N1che.ServiceTests.Infrastructure.Google;

// The route Google is standing by to compute. Everything a computed route reports comes from here, so
// a scenario asserts the response against this rather than against anything it posted.
internal sealed record GoogleRoute
{
    public required double DistanceMeters { get; init; }

    public required int DurationSeconds { get; init; }

    public required IReadOnlyCollection<CoordinateResponse> Polyline { get; init; }

    public IReadOnlyCollection<GoogleLeg> Legs { get; init; } = [];
}
