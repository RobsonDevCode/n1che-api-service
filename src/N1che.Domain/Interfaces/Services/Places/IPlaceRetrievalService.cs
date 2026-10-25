using N1che.Domain.Models.Places;

namespace N1che.Domain.Interfaces.Services.Places;

/// <summary>Retrieves places for the places endpoints.</summary>
public interface IPlaceRetrievalService
{
    /// <summary>
    /// Finds the places inside the filter's box matching its text, empty when none do. Throws
    /// <see cref="Exceptions.GooglePlacesException"/> when Google fails to answer.
    /// </summary>
    Task<IReadOnlyCollection<PlaceModel>> SearchAsync(PlacesSearchFilterModel filter, CancellationToken cancellationToken);

    /// <summary>
    /// Gets a short-lived public address for a place photo's image. Throws
    /// <see cref="Exceptions.NotFoundException"/> when Google has no photo with that reference,
    /// <see cref="Exceptions.InvalidRequestException"/> when Google will not accept it, and
    /// <see cref="Exceptions.GooglePlacesException"/> when Google fails to answer.
    /// </summary>
    Task<PlacePhotoMediaModel> GetPhotoMediaAsync(PlacePhotoMediaFilterModel filter, CancellationToken cancellationToken);
}
