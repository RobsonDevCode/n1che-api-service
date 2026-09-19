using N1che.Contracts.Response.Places;
using N1che.Domain.Models.Places;

namespace N1che.Api.Extensions.Places;

public static class PlaceResponseExtensions
{
    public static IReadOnlyCollection<PlaceResponse> ToResponse(this IReadOnlyCollection<PlaceModel> places) =>
        places.Select(place => place.ToResponse()).ToArray();

    private static PlaceResponse ToResponse(this PlaceModel place) => new()
    {
        GooglePlaceId = place.GooglePlaceId,
        Name = place.Name,
        Address = place.Address,
        Latitude = place.Latitude,
        Longitude = place.Longitude,
        PhotoReference = place.PhotoReference,
    };
}
