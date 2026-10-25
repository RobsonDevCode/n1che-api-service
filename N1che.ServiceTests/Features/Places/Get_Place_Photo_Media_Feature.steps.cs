using System.Net;
using System.Net.Http.Json;
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

public partial class Get_Place_Photo_Media_Feature : FeatureFixture
{
    private const string SuccessLog = "Photo retrieved";
    private const string GoogleUnavailableMessage = "Google Places is currently unavailable.";

    private const string PlaceIdField = "PlaceId";
    private const string PhotoIdField = "PhotoId";
    private const string MaxWidthField = "MaxWidth";

    private const string PlaceIdInvalidMessage = "PlaceId must contain only letters, digits, '-' and '_'.";
    private const string PhotoIdInvalidMessage = "PhotoId must contain only letters, digits, '-' and '_'.";
    private const string MaxWidthOutOfRangeMessage = "MaxWidth must be between 1 and 4800.";

    // The width the app asks for its shop panel hero.
    private const int MaxWidth = 400;

    // Google's largest accepted width is 4800.
    private const int OversizedWidth = 4801;

    // '~' is allowed in a URL path but is not part of any Google id.
    private const string InvalidId = "not~an~id";

    // Google serves a key it will not accept with this.
    private const HttpStatusCode RejectedApiKeyStatus = HttpStatusCode.Forbidden;

    // Only logs written through the logger that opened the scope carry it, so the client's error and
    // the exception handler's warning — raised beneath the endpoint — do not.
    private static readonly Dictionary<string, object> NoScopes = [];

    // The ids are unique to this fixture, so no scenario matches another's stubbed photo.
    private readonly string _reference;
    private readonly string _photoUri = $"https://lh3.googleusercontent.com/place-photos/{Guid.NewGuid()}=s1600-w{MaxWidth}";

    private readonly PlacePhotoMediaFilter _filter;
    private readonly PlacePhotoMediaFilter _invalidPlaceIdFilter;
    private readonly PlacePhotoMediaFilter _newlinePlaceIdFilter;
    private readonly PlacePhotoMediaFilter _invalidPhotoIdFilter;
    private readonly PlacePhotoMediaFilter _zeroWidthFilter;
    private readonly PlacePhotoMediaFilter _oversizedWidthFilter;

    private readonly string _endpointLog;
    private readonly string _notFoundMessage;
    private readonly string _rejectedMessage;
    private readonly string _googleRejectedLog;
    private readonly string _googleFailureLog;
    private readonly Dictionary<string, object> _scopeValues;

    private HttpResponseMessage _response;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    public Get_Place_Photo_Media_Feature()
    {
        _filter = new PlacePhotoMediaFilter
        {
            PlaceId = $"place-{Guid.NewGuid()}",
            PhotoId = $"photo-{Guid.NewGuid()}",
            MaxWidth = MaxWidth,
        };

        _invalidPlaceIdFilter = _filter with { PlaceId = InvalidId };
        _newlinePlaceIdFilter = _filter with { PlaceId = $"{_filter.PlaceId}\n" };
        _invalidPhotoIdFilter = _filter with { PhotoId = InvalidId };
        _zeroWidthFilter = _filter with { MaxWidth = 0 };
        _oversizedWidthFilter = _filter with { MaxWidth = OversizedWidth };

        _reference = $"places/{_filter.PlaceId}/photos/{_filter.PhotoId}";

        _endpointLog = $"Getting photo {_reference}";
        _notFoundMessage = $"Photo {_reference} not found";
        _rejectedMessage = $"Photo {_reference} cannot be loaded.";
        _googleRejectedLog =
            $"Google Places answered {(int)HttpStatusCode.BadRequest} for photo {_reference}: {GooglePlacesMock.ErrorBody}";
        _googleFailureLog =
            $"Google Places answered {(int)RejectedApiKeyStatus} for photo {_reference}: {GooglePlacesMock.ErrorBody}";
        _scopeValues = new Dictionary<string, object> { ["PhotoReference"] = _reference };
    }

    private Task Google_Returns_The_Photo_Media()
    {
        GooglePlacesMock.ReturnsPhotoMedia(_reference, MaxWidth, _photoUri);
        return Task.CompletedTask;
    }

    private Task Google_Answers_With(HttpStatusCode statusCode)
    {
        GooglePlacesMock.AnswersPhotoMediaWith(_reference, MaxWidth, statusCode);
        return Task.CompletedTask;
    }

    private async Task GetPlacePhotoMedia_Is_Called(PlacePhotoMediaFilter filter)
    {
        _response = await Client.GetPlacePhotoMedia(filter);
    }

    private async Task The_Photo_Uri_Is_Returned()
    {
        var media = await _response.Content.ReadFromJsonAsync<PlacePhotoMediaResponse>();

        media.Should().BeEquivalentTo(new PlacePhotoMediaResponse { PhotoUri = _photoUri });
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = $"/{_reference}/media"
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
}
