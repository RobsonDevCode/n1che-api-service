using N1che.Domain.Models.Places;

namespace N1che.Domain.Interfaces.ThirdParty;

/// <summary>Reads place detail from Google Places.</summary>
public interface IGooglePlacesClient
{
    /// <summary>
    /// Gets the detail Google holds for a place, or <c>null</c> when Google has no place with that
    /// identifier, will not accept it, or holds too little of the place to add it. Throws
    /// <see cref="Exceptions.GooglePlacesException"/> when Google itself fails to answer.
    /// </summary>
    Task<PlaceDetailsModel?> GetPlaceDetails(string googlePlaceId, CancellationToken cancellationToken);
}
