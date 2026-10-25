namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>A GeoJSON LineString: each coordinate is a <c>[longitude, latitude]</c> pair.</summary>
public sealed record GeoJsonLineStringResponse
{
    public IReadOnlyCollection<IReadOnlyCollection<double>>? Coordinates { get; init; }
}
