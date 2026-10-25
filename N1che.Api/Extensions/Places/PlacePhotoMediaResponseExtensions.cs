using N1che.Contracts.Response.Places;
using N1che.Domain.Models.Places;

namespace N1che.Api.Extensions.Places;

public static class PlacePhotoMediaResponseExtensions
{
    public static PlacePhotoMediaResponse ToResponse(this PlacePhotoMediaModel media) => new()
    {
        PhotoUri = media.PhotoUri,
    };
}
