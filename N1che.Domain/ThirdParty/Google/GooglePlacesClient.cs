using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Places;
using N1che.Domain.ThirdParty.Google.Extensions;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google;

public sealed class GooglePlacesClient : IGooglePlacesClient
{
    private const string FieldMaskHeader = "X-Goog-FieldMask";

    private const string PlaceDetailsFieldMask =
        "businessStatus,displayName,formattedAddress,location,regularOpeningHours";

    private const string PlaceSearchFieldMask =
        "places.id,places.displayName,places.formattedAddress,places.location,places.photos";

    private readonly HttpClient _httpClient;
    private readonly ILogger<GooglePlacesClient> _logger;

    public GooglePlacesClient(HttpClient httpClient, ILogger<GooglePlacesClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PlaceDetailsModel?> GetPlaceDetails(string googlePlaceId, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"v1/places/{Uri.EscapeDataString(googlePlaceId)}");
        request.Headers.Add(FieldMaskHeader, PlaceDetailsFieldMask);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        // Google answers an id it cannot parse with 400 and one it holds no place for with 404, so
        // both say the caller named something that cannot be added rather than that Google is down.
        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError("Google Places answered {StatusCode} for place {GooglePlaceId}: {Error}",
                (int)response.StatusCode, googlePlaceId, error);

            throw new GooglePlacesException();
        }

        var details = await response.Content.ReadFromJsonAsync<PlaceDetailsResponse>(cancellationToken);

        return details?.ToDomainModel();
    }

    public async Task<IReadOnlyCollection<PlaceModel>> SearchPlaces(
        PlacesSearchFilterModel filter, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "v1/places:searchText")
        {
            Content = JsonContent.Create(filter.ToGoogleRequest())
        };
        request.Headers.Add(FieldMaskHeader, PlaceSearchFieldMask);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        // Unlike a place id, a search Google rejects is not something the caller can be told to fix —
        // the box and text were validated before they got here — so every failure is Google's.
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError("Google Places answered {StatusCode} for search {Query}: {Error}",
                (int)response.StatusCode, filter.Query, error);

            throw new GooglePlacesException();
        }

        var results = await response.Content.ReadFromJsonAsync<PlaceSearchResponse>(cancellationToken);

        return results?.ToDomainModels() ?? [];
    }
}
