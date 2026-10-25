namespace N1che.Domain.ThirdParty.Google.Requests;

public sealed record RouteWaypointLocationRequest
{
    public required PointRequest LatLng { get; init; }
}
