using DomainFilter = N1che.Domain.Models.Shops.NearbyShopsFilter;
using ContractFilter = N1che.Contracts.Filters.Shops.NearbyShopsFilter;

namespace N1che.Api.Extensions.Shops;

public static class NearbyShopsFilterExtensions
{
    private const double DefaultRadiusMeters = 5000;
    private const int DefaultLimit = 50;

    public static DomainFilter ToDomainFilter(this ContractFilter filter) => new()
    {
        Latitude = filter.Lat,
        Longitude = filter.Lng,
        RadiusMeters = filter.Radius ?? DefaultRadiusMeters,
        Niche = filter.Niche,
        Limit = filter.Limit ?? DefaultLimit,
    };
}
