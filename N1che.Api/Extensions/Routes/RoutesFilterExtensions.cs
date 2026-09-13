using N1che.Contracts.Filters.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class RoutesFilterExtensions
{
    private const double DefaultRadiusMeters = 5000;
    private const int DefaultLimit = 20;

    public static RoutesFilterModel ToDomainFilter(this RoutesFilter filter) => new()
    {
        Latitude = filter.Lat,
        Longitude = filter.Lng,
        RadiusMeters = filter.Radius ?? DefaultRadiusMeters,
        Niche = filter.Niche,
        Limit = filter.Limit ?? DefaultLimit,
    };
}
