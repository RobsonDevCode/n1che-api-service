namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>One point of a route: where it is, wrapped as Google nests a waypoint's location.</summary>
public sealed record RouteWaypointRequest
{
    public required RouteWaypointLocationRequest Location { get; init; }
}
