namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>One point of a route.</summary>
public sealed record RouteWaypointRequest
{
    public required RouteWaypointLocationRequest Location { get; init; }
}
