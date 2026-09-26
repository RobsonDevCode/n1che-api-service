namespace N1che.Domain.Models.Routes;

/// <summary>A route to compute the walk for.</summary>
public record ComputeRouteModel
{
    /// <summary>The shops to walk through, in the order they are walked.</summary>
    public required IReadOnlyCollection<Guid> StopIds { get; init; }

    /// <summary>Where the walk starts, absent when it starts at the first stop.</summary>
    public CoordinateModel? Origin { get; init; }

    /// <summary>One of <see cref="RouteModes"/>.</summary>
    public required string Mode { get; init; }
}
