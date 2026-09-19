using N1che.Domain.Models.Places;

namespace N1che.Domain.Interfaces.ThirdParty;

/// <summary>Reads places and place detail from Google Places.</summary>
public interface IGooglePlacesClient
{
    /// <summary>
    /// Gets the detail Google holds for a place, or <c>null</c> when Google has no place with that
    /// identifier, will not accept it, or holds too little of the place to add it. Throws
    /// <see cref="Exceptions.GooglePlacesException"/> when Google itself fails to answer.
    /// </summary>
    Task<PlaceDetailsModel?> GetPlaceDetails(string googlePlaceId, CancellationToken cancellationToken);

    /// <summary>
    /// Finds the places inside the filter's box matching its text, empty when none do. Places Google
    /// holds too little of to add as a shop are dropped. Throws
    /// <see cref="Exceptions.GooglePlacesException"/> when Google fails to answer.
    /// </summary>
    Task<IReadOnlyCollection<PlaceModel>> SearchPlaces(PlacesSearchFilterModel filter, CancellationToken cancellationToken);
}
