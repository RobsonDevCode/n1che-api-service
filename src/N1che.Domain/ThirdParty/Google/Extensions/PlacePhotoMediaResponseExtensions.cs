using N1che.Domain.Models.Places;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class PlacePhotoMediaResponseExtensions
{
    /// <summary>Null when Google answered without an address for the image.</summary>
    public static PlacePhotoMediaModel? ToDomainModel(this PlacePhotoMediaResponse response) =>
        string.IsNullOrWhiteSpace(response.PhotoUri)
            ? null
            : new PlacePhotoMediaModel { PhotoUri = response.PhotoUri };
}
