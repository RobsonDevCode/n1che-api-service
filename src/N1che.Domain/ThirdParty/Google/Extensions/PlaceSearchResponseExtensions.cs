using N1che.Domain.Models.Places;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class PlaceSearchResponseExtensions
{
    /// <summary>Places Google holds too little of to build a shop from are dropped rather than returned half-empty.</summary>
    public static IReadOnlyCollection<PlaceModel> ToDomainModels(this PlaceSearchResponse response) =>
        (response.Places ?? [])
            .Select(ToDomainModel)
            .OfType<PlaceModel>()
            .ToArray();

    private static PlaceModel? ToDomainModel(PlaceSummaryResponse place)
    {
        var name = place.DisplayName?.Text;
        if (string.IsNullOrWhiteSpace(place.Id)
            || string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(place.FormattedAddress)
            || place.Location is null)
        {
            return null;
        }

        return new PlaceModel
        {
            GooglePlaceId = place.Id,
            Name = name,
            Address = place.FormattedAddress,
            Latitude = place.Location.Latitude,
            Longitude = place.Location.Longitude,
            Photo = ToPhoto(place.Photos?.FirstOrDefault()),
        };
    }

    // Google's terms require every photo to link to its source on Google Maps, so one without that link
    // cannot be shown and is dropped. An author with no name cannot be credited, so is dropped too.
    private static PlacePhotoModel? ToPhoto(PlacePhotoResponse? photo)
    {
        if (string.IsNullOrWhiteSpace(photo?.Name) || string.IsNullOrWhiteSpace(photo.GoogleMapsUri))
        {
            return null;
        }

        return new PlacePhotoModel
        {
            Reference = photo.Name,
            GoogleMapsUri = photo.GoogleMapsUri,
            AuthorAttributions = (photo.AuthorAttributions ?? [])
                .Where(author => !string.IsNullOrWhiteSpace(author.DisplayName))
                .Select(author => new PhotoAuthorAttributionModel
                {
                    DisplayName = author.DisplayName!,
                    Uri = author.Uri,
                    PhotoUri = author.PhotoUri,
                })
                .ToArray(),
        };
    }
}
