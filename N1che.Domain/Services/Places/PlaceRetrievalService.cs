using N1che.Domain.Interfaces.Services.Places;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Places;

namespace N1che.Domain.Services.Places;

public sealed class PlaceRetrievalService : IPlaceRetrievalService
{
    private readonly IGooglePlacesClient _googlePlacesClient;

    public PlaceRetrievalService(IGooglePlacesClient googlePlacesClient)
    {
        _googlePlacesClient = googlePlacesClient;
    }

    public Task<IReadOnlyCollection<PlaceModel>> SearchAsync(
        PlacesSearchFilterModel filter, CancellationToken cancellationToken)
    {
        return _googlePlacesClient.SearchPlaces(filter, cancellationToken);
    }
}
