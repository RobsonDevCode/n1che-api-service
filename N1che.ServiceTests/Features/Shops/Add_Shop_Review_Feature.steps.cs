using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Requests.Reviews;
using N1che.Contracts.Response.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Add_Shop_Review_Feature : FeatureFixture
{
    private const string SuccessLog = "Shop review added";
    private const string ValidationTitle = "One or more validation errors occurred.";

    private const string TextField = "Text";
    private const string NicheField = "Niche";

    private const string TextValidationMessage = "Text is required.";
    private const string TextTooLongValidationMessage = "Text must be at most 2000 characters.";
    private const string NicheValidationMessage = "Niche is required.";

    private const int TooLongTextLength = 2001;

    private readonly IFixture _fixture;

    // Reviews are keyed on their shop, so a fresh shop per scenario keeps every other scenario's rows out.
    private readonly ShopEntity _shop;
    private readonly Guid _unknownShopId = Guid.NewGuid();

    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _callerUsername = $"user-{Guid.NewGuid()}";

    private readonly CreateReviewRequest _request;
    private readonly CreateReviewRequest _otherNicheRequest;
    private readonly CreateReviewRequest _textlessRequest;
    private readonly CreateReviewRequest _tooLongRequest;
    private readonly CreateReviewRequest _nicheLessRequest;

    private readonly string _otherNicheMessage;
    private readonly string _notFoundMessage;

    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId, _callerUsername));

    public Add_Shop_Review_Feature()
    {
        _fixture = new Fixture();

        _shop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: [NicheConstants.Goth, NicheConstants.Vintage]);

        _request = new CreateReviewRequest
        {
            Text = _fixture.Create<string>(),
            Niche = NicheConstants.Goth,
        };

        _otherNicheRequest = _request with { Niche = NicheConstants.Skater };
        _textlessRequest = _request with { Text = string.Empty };
        _tooLongRequest = _request with { Text = new string('a', TooLongTextLength) };
        _nicheLessRequest = _request with { Niche = string.Empty };

        _otherNicheMessage = $"Shop {_shop.Id} is not listed under niche {NicheConstants.Skater}.";
        _notFoundMessage = $"Shop {_unknownShopId} not found";
    }

    private async Task The_Shop_Exists()
    {
        await ShopPersistenceProvider.Insert([_shop]);
    }

    private async Task CreateShopReview_Is_Called(Guid shopId, CreateReviewRequest request)
    {
        _endpointLog = $"Adding review for shop {shopId} in niche {request.Niche}";
        _scopeValues = new Dictionary<string, object>
        {
            ["ShopId"] = shopId,
            ["Niche"] = request.Niche,
            ["UserId"] = _callerId
        };

        _response = await Client.CreateShopReview(shopId, request);
    }

    private async Task The_Created_Review_Is_Returned_And_Stored()
    {
        var stored = (await ReviewsPersistenceProvider.GetByShopId(_shop.Id)).Should().ContainSingle().Subject;
        var created = await _response.Content.ReadFromJsonAsync<ReviewResponse>();

        created.Should().BeEquivalentTo(new ReviewResponse
        {
            Id = stored.Id,
            ShopId = _shop.Id,
            Niche = _request.Niche,
            UserId = _callerId,
            Username = _callerUsername,
            Text = _request.Text,
            CreatedAt = stored.CreatedAt,
            UpdatedAt = stored.UpdatedAt,
        }, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());

        stored.Body.Should().Be(_request.Text);
        stored.UpdateCount.Should().Be(0);
        _response.Headers.Location.Should().Be($"/shops/{_shop.Id}/reviews");
    }

    private async Task No_Review_Is_Stored(Guid shopId)
    {
        var reviews = await ReviewsPersistenceProvider.GetByShopId(shopId);

        reviews.Should().BeEmpty();
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message, Guid shopId)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = $"/shops/{shopId}/reviews"
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
            Title = ValidationTitle
        }, options => options
            .Excluding(details => details.Extensions)
            .Excluding(details => details.Type));
    }
}
