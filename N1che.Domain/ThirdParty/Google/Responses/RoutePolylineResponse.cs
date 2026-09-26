namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>
/// A polyline as Google returns it. Which member is filled follows the <c>polylineEncoding</c> asked
/// for.
/// </summary>
public sealed record RoutePolylineResponse
{
    public GeoJsonLineStringResponse? GeoJsonLinestring { get; init; }
}
