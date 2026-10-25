namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>A point in decimal degrees (WGS 84), as Google's request bodies take it.</summary>
public sealed record PointRequest
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }
}
