using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Filters.Places;
using N1che.Contracts.Response.Places;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Google;
using N1che.ServiceTests.Infrastructure.Logger;

namespace N1che.ServiceTests.Features.Places;

public partial class Search_Places_Feature : FeatureFixture
{
    private const string SuccessLog = "Places retrieved";
    private const string GoogleUnavailableMessage = "Google Places is currently unavailable.";

    private const string QueryField = "Query";
    private const string NeLatField = "NeLat";
    private const string NeLngField = "NeLng";

    private const string QueryRequiredMessage = "Query is required.";
    private const string CornerNotNorthMessage = "The north-east corner must be north of the south-west corner.";
    private const string CornerNotEastMessage = "The north-east corner must be east of the south-west corner.";
    private const string TooTallMessage = "The search area must span at most 2 degrees of latitude.";
    private const string TooWideMessage = "The search area must span at most 2 degrees of longitude.";

    // Google serves a key it will not accept with this.
    private const HttpStatusCode RejectedApiKeyStatus = HttpStatusCode.Forbidden;

    // A box over central London. The area is the caller's to choose, so it stays fixed across scenarios
    // and the query is what keeps each scenario's stubbed search apart.
    private const double SouthWestLatitude = 51.4;
    private const double SouthWestLongitude = -0.2;
    private const double NorthEastLatitude = 51.6;
    private const double NorthEastLongitude = -0.05;

    // Degrees past the allowed span, for the scenarios that ask for too large an area.
    private const double OversizedSpan = 3;

    // Only logs written through the logger that opened the scope carry it, so the client's own error —
    // raised beneath the endpoint — does not.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The query is unique to this fixture, so no scenario matches another's stubbed search.
    private readonly string _query = $"query-{Guid.NewGuid()}";

    private readonly PlacesSearchFilter _filter;
    private readonly PlacesSearchFilter _queryLessFilter;
    private readonly PlacesSearchFilter _blankQueryFilter;
    private readonly PlacesSearchFilter _flatAreaFilter;
    private readonly PlacesSearchFilter _narrowAreaFilter;
    private readonly PlacesSearchFilter _tallAreaFilter;
    private readonly PlacesSearchFilter _wideAreaFilter;

    private readonly GoogleSearchResult _photographedPlace;
    private readonly GoogleSearchResult _unphotographedPlace;
    private readonly GoogleSearchResult _addresslessPlace;

    private readonly string _endpointLog;
    private readonly string _googleFailureLog;
    private readonly Dictionary<string, object> _scopeValues;

    private HttpResponseMessage _response;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    public Search_Places_Feature()
    {
        _fixture = new Fixture();

        _filter = new PlacesSearchFilter
        {
            Query = _query,
            SwLat = SouthWestLatitude,
            SwLng = SouthWestLongitude,
            NeLat = NorthEastLatitude,
            NeLng = NorthEastLongitude,
        };

        _queryLessFilter = _filter with { Query = null! };
        _blankQueryFilter = _filter with { Query = "" };
        _flatAreaFilter = _filter with { NeLat = SouthWestLatitude };
        _narrowAreaFilter = _filter with { NeLng = SouthWestLongitude };
        _tallAreaFilter = _filter with { NeLat = SouthWestLatitude + OversizedSpan };
        _wideAreaFilter = _filter with { NeLng = SouthWestLongitude + OversizedSpan };

        _photographedPlace = PlaceInTheArea() with { PhotoName = $"places/{Guid.NewGuid()}/photos/{Guid.NewGuid()}" };
        _unphotographedPlace = PlaceInTheArea();
        _addresslessPlace = PlaceInTheArea() with { Address = null };

        _endpointLog = $"Searching places for {_query}";
        _googleFailureLog =
            $"Google Places answered {(int)RejectedApiKeyStatus} for search {_query}: {GooglePlacesMock.ErrorBody}";
        _scopeValues = new Dictionary<string, object> { ["Query"] = _query };
    }

    private Task Google_Returns_Successfully(params GoogleSearchResult[] places)
    {
        GooglePlacesMock.ReturnsSearchResults(_filter, places);
        return Task.CompletedTask;
    }

    private Task Google_Returns_Unsuccessfully()
    {
        GooglePlacesMock.AnswersSearchWith(_filter, RejectedApiKeyStatus);
        return Task.CompletedTask;
    }

    private async Task SearchPlaces_Is_Called(PlacesSearchFilter filter)
    {
        _response = await Client.SearchPlaces(filter);
    }

    private async Task The_Places_Are_Returned(params GoogleSearchResult[] expected)
    {
        var places = await _response.Content.ReadFromJsonAsync<IReadOnlyCollection<PlaceResponse>>();

        places.Should().BeEquivalentTo(expected.Select(place => new PlaceResponse
        {
            GooglePlaceId = place.GooglePlaceId,
            Name = place.Name,
            Address = place.Address!,
            Latitude = place.Latitude,
            Longitude = place.Longitude,
            PhotoReference = place.PhotoName,
        }), options => options.WithStrictOrdering());
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = "/places/search"
        }, options => options.Excluding(details => details.Extensions));
    }

    private async Task The_Response_Is_A_Validation_Error_For(string field, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();

        problem.Should().BeEquivalentTo(new HttpValidationProblemDetails(new Dictionary<string, string[]>
        {
            [field] = [message]
        })
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "One or more validation errors occurred."
        }, options => options
            .Excluding(details => details.Extensions)
            .Excluding(details => details.Type));
    }

    private GoogleSearchResult PlaceInTheArea() => new()
    {
        GooglePlaceId = $"place-{Guid.NewGuid()}",
        Name = _fixture.Create<string>(),
        Address = _fixture.Create<string>(),
        Latitude = SouthWestLatitude + Random.Shared.NextDouble() * (NorthEastLatitude - SouthWestLatitude),
        Longitude = SouthWestLongitude + Random.Shared.NextDouble() * (NorthEastLongitude - SouthWestLongitude),
    };
}
