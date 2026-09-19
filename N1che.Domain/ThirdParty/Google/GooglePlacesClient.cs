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
            _logger.LogError("Google Places answered {StatusCode} for place {GooglePlaceId}",
                (int)response.StatusCode, googlePlaceId);

            throw new GooglePlacesException();
        }

        var details = await response.Content.ReadFromJsonAsync<PlaceDetailsResponse>(cancellationToken);

        return details?.ToDomainModel();
    }
}
