namespace N1che.Domain.Models.Routes;

/// <summary>A route to save, as the user built it and as the walk was computed for them.</summary>
public record CreateRouteModel
{
    public required string Name { get; init; }

    public required string Tag { get; init; }

    public required string Niche { get; init; }

    /// <summary>The shops to walk through, in the order they are walked.</summary>
    public required IReadOnlyList<Guid> StopIds { get; init; }

    public required IReadOnlyCollection<CoordinateModel> Polyline { get; init; }

    public required double DistanceMeters { get; init; }

    public required int TotalMinutes { get; init; }

    public required string CreatedByUserId { get; init; }

    public required string CreatedByUsername { get; init; }
}
