using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Requests.Shops;
using N1che.Contracts.Response.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Google;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Add_Shop_Feature : FeatureFixture
{
    private const string SuccessLog = "Shop added";
    private const string OperationalPlaceStatus = "operational";
    private const string UnrecognisedNiche = "not-a-niche";
    private const string UnrecognisedNicheMessage = $"Niche {UnrecognisedNiche} is not recognised.";
    private const string UnrecognisedNicheLog = $"Niche {UnrecognisedNiche} is not recognised";
    private const string NichesField = "Niches";
    private const string NichesValidationMessage = "At least one niche is required.";
    private const string GoogleUnavailableMessage = "Google Places is currently unavailable.";

    // Google answers an id it cannot parse with this, and a key it will not serve with the other.
    private const HttpStatusCode RejectedPlaceIdStatus = HttpStatusCode.BadRequest;
    private const HttpStatusCode RejectedApiKeyStatus = HttpStatusCode.Forbidden;

    private static readonly TimeOnly EndOfDay = new(23, 59, 59);

    private static readonly TradingPeriod TradesToday =
        TradingPeriod.Today(new TimeOnly(9, 0), new TimeOnly(17, 30));

    private static readonly TradingPeriod MorningToday =
        TradingPeriod.Today(new TimeOnly(9, 0), new TimeOnly(13, 0));

    private static readonly TradingPeriod AfternoonToday =
        TradingPeriod.Today(new TimeOnly(14, 0), new TimeOnly(18, 0));

    // The two periods above collapse into this single window, which is all shop_hours can hold.
    private static readonly TradingPeriod TradesAllDayToday =
        TradingPeriod.Today(MorningToday.OpenTime, AfternoonToday.CloseTime);

    // Google closes this one on tomorrow, so only the part before midnight survives the collapse.
    private static readonly TradingPeriod TradesPastMidnightToday =
        TradingPeriod.Today(new TimeOnly(20, 0), new TimeOnly(2, 0));

    private static readonly TradingPeriod TradesUntilMidnightToday =
        TradingPeriod.Today(TradesPastMidnightToday.OpenTime, EndOfDay);

    // Only logs written through the logger that opened the scope carry it, so neither the domain
    // service's own warnings nor the handler's — raised after the endpoint's scope is disposed — do.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The place, the caller and the shop name are unique to this fixture, so scenarios never see
    // each other's rows or each other's stubbed places.
    private readonly string _googlePlaceId = $"place-{Guid.NewGuid()}";
    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _callerUsername = $"user-{Guid.NewGuid()}";

    private readonly GooglePlace _place;
    private readonly CreateShopRequest _request;
    private readonly CreateShopRequest _unrecognisedNicheRequest;
    private readonly CreateShopRequest _nicheLessRequest;
    private readonly string _endpointLog;
    private readonly Dictionary<string, object> _scopeValues;

    private readonly string _duplicateShopMessage;
    private readonly string _rejectedPlaceMessage;
    private readonly string _unknownPlaceLog;
    private readonly string _permanentlyClosedLog;
    private readonly string _googleFailureLog;

    private HttpResponseMessage _response;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId, _callerUsername));

    public Add_Shop_Feature()
    {
        _fixture = new Fixture();

        _place = new GooglePlace
        {
            GooglePlaceId = _googlePlaceId,
            BusinessStatus = GooglePlacesMock.OperationalStatus,
            Name = _fixture.Create<string>(),
            Address = _fixture.Create<string>(),
            // Coordinates stay in range so the geography cast doesn't silently coerce them.
            Latitude = Random.Shared.Next(-80, 80) + Random.Shared.NextDouble(),
            Longitude = Random.Shared.Next(-170, 170) + Random.Shared.NextDouble(),
        };

        _request = new CreateShopRequest
        {
            GooglePlaceId = _googlePlaceId,
            Niches = [NicheConstants.Goth, NicheConstants.Vintage]
        };

        _unrecognisedNicheRequest = _request with { Niches = [UnrecognisedNiche] };
        _nicheLessRequest = _request with { Niches = [] };

        _endpointLog = $"Adding shop for google place {_googlePlaceId}";
        _scopeValues = new Dictionary<string, object>
        {
            ["GooglePlaceId"] = _googlePlaceId,
            ["UserId"] = _callerId
        };

        _duplicateShopMessage = $"Shop {_googlePlaceId} already exists in the database.";
        _rejectedPlaceMessage = $"Google place {_googlePlaceId} cannot be added.";
        _unknownPlaceLog = $"Google has no place {_googlePlaceId}";
        _permanentlyClosedLog = $"Google place {_googlePlaceId} is permanently closed";
        _googleFailureLog =
            $"Google Places answered {(int)RejectedApiKeyStatus} for place {_googlePlaceId}: {GooglePlacesMock.ErrorBody}";
    }

    private Task The_Place_Trades(params TradingPeriod[] periods)
    {
        GooglePlacesMock.ReturnsPlaceDetails(_place with { Periods = periods });
        return Task.CompletedTask;
    }

    private Task The_Place_Is_Permanently_Closed()
    {
        GooglePlacesMock.ReturnsPlaceDetails(_place with { BusinessStatus = GooglePlacesMock.PermanentlyClosedStatus });
        return Task.CompletedTask;
    }

    private Task Google_Will_Not_Accept_The_Place_Id()
    {
        GooglePlacesMock.AnswersWith(_googlePlaceId, RejectedPlaceIdStatus);
        return Task.CompletedTask;
    }

    private Task Google_Rejects_Our_Api_Key()
    {
        GooglePlacesMock.AnswersWith(_googlePlaceId, RejectedApiKeyStatus);
        return Task.CompletedTask;
    }

    private async Task CreateShop_Is_Called(CreateShopRequest request)
    {
        _response = await Client.CreateShop(request);
    }

    private async Task The_Created_Shop_Is_Returned(TimeOnly? openTime, TimeOnly? closeTime)
    {
        var stored = await ShopPersistenceProvider.GetByGooglePlaceId(_googlePlaceId);
        var shop = await _response.Content.ReadFromJsonAsync<ShopDetailResponse>();

        shop.Should().BeEquivalentTo(new ShopDetailResponse
        {
            Id = stored.Id.ToString(),
            GooglePlaceId = _googlePlaceId,
            Name = _place.Name,
            Niches = _request.Niches,
            Address = _place.Address,
            Latitude = _place.Latitude,
            Longitude = _place.Longitude,
            PlaceStatus = OperationalPlaceStatus,
            OpenTime = openTime,
            CloseTime = closeTime,
            CreatedAt = stored.CreatedAt,
            AddedByUserId = _callerId,
            AddedByUsername = _callerUsername,
            PhotoUrl = null
        }, options => options
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>());

        _response.Headers.Location.Should().Be($"/shops/{stored.Id}");
    }

    private async Task The_Stored_Trading_Hours_Are(params TradingPeriod[] expected)
    {
        var stored = await ShopPersistenceProvider.GetByGooglePlaceId(_googlePlaceId);
        var hours = await ShopPersistenceProvider.GetHours(stored.Id);

        hours.Should().BeEquivalentTo(expected, options => options.ExcludingMissingMembers());
    }

    private async Task No_Shop_Is_Stored()
    {
        var count = await ShopPersistenceProvider.CountByGooglePlaceId(_googlePlaceId);

        count.Should().Be(0);
    }

    private async Task One_Shop_Is_Stored()
    {
        var count = await ShopPersistenceProvider.CountByGooglePlaceId(_googlePlaceId);

        count.Should().Be(1);
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = "/shops"
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
