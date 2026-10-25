namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>Where Google puts the place, in decimal degrees (WGS 84).</summary>
public sealed record PlaceLocationResponse
{
    public double Latitude { get; init; }

    public double Longitude { get; init; }
}
