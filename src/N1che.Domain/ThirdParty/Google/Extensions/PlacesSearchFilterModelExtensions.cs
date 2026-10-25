using N1che.Domain.Models.Places;
using N1che.Domain.ThirdParty.Google.Requests;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class PlacesSearchFilterModelExtensions
{
    // Google's per-page maximum. A search is billed per request rather than per result and nothing here
    // pages, so asking for fewer would cost the same and only hide matches the caller never learns of.
    private const int SearchPageSize = 20;

    public static PlaceSearchRequest ToGoogleRequest(this PlacesSearchFilterModel filter) => new()
    {
        TextQuery = filter.Query,
        PageSize = SearchPageSize,
        LocationRestriction = new PlaceLocationRestrictionRequest
        {
            Rectangle = new PlaceRectangleRequest
            {
                Low = new PointRequest
                {
                    Latitude = filter.SouthWestLatitude,
                    Longitude = filter.SouthWestLongitude,
                },
                High = new PointRequest
                {
                    Latitude = filter.NorthEastLatitude,
                    Longitude = filter.NorthEastLongitude,
                }
            }
        }
    };
}
