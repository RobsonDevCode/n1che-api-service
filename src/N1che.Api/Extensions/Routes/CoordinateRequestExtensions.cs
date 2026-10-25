using N1che.Contracts.Requests.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class CoordinateRequestExtensions
{
    public static CoordinateModel ToDomainModel(this CoordinateRequest coordinate) => new()
    {
        Latitude = coordinate.Latitude,
        Longitude = coordinate.Longitude,
    };
}
