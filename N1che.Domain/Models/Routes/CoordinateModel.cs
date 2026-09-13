namespace N1che.Domain.Models.Routes;

public record CoordinateModel
{
    public required double Latitude { get; init; }

    public required double Longitude { get; init; }
}
