using N1che.Contracts.Filters.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class GetRouteFilterExtensions
{
    public static CoordinateModel ToDomainModel(this GetRouteFilter filter) => new()
    {
        Latitude = filter.Lat,
        Longitude = filter.Lng,
    };
}
