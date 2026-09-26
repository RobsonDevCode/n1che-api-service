namespace N1che.Contracts.Requests.Routes;

public record ComputeRouteRequest
{
    /// <summary>The shops to walk through, in the order they are walked.</summary>
    public required IReadOnlyCollection<Guid> Stops { get; init; }

    /// <summary>Where the walk starts, absent when it starts at the first stop.</summary>
    public CoordinateRequest? Origin { get; init; }

    /// <summary><c>you</c> to walk the stops in order, <c>loop</c> to return to the first.</summary>
    public required string Mode { get; init; }
}
