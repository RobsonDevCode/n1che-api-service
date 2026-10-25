namespace N1che.Contracts.Response.Routes;

public record CoordinateResponse
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }
}
