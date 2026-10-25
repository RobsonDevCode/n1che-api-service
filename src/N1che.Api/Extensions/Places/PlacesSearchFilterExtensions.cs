using N1che.Contracts.Filters.Places;
using N1che.Domain.Models.Places;

namespace N1che.Api.Extensions.Places;

public static class PlacesSearchFilterExtensions
{
    public static PlacesSearchFilterModel ToDomainFilter(this PlacesSearchFilter filter) => new()
    {
        Query = filter.Query,
        SouthWestLatitude = filter.SwLat,
        SouthWestLongitude = filter.SwLng,
        NorthEastLatitude = filter.NeLat,
        NorthEastLongitude = filter.NeLng,
    };
}
