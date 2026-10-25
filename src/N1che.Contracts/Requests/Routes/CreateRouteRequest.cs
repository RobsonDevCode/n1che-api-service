namespace N1che.Contracts.Requests.Routes;

public record CreateRouteRequest
{
    public required string Name { get; init; }

    public required string Tag { get; init; }

    /// <summary>The niche the route belongs to.</summary>
    public required string Niche { get; init; }

    /// <summary>The shops to walk through, in the order they are walked.</summary>
    public required IReadOnlyList<Guid> Stops { get; init; }

    /// <summary>The line the walk follows, as <c>POST /routes/compute</c> returned it for these stops.</summary>
    public required IReadOnlyCollection<CoordinateRequest> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }
}
