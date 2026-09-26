namespace N1che.Contracts.Requests.Routes;

/// <summary>A point in decimal degrees (WGS 84).</summary>
public record CoordinateRequest
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }
}
