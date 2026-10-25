using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Requests.Reviews;
using N1che.Contracts.Response.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Update_Shop_Review_Feature : FeatureFixture
{
    private const string SuccessLog = "Shop review updated";
    private const string ValidationTitle = "One or more validation errors occurred.";

    private const string TextField = "Text";

    private const string TextValidationMessage = "Text is required.";
    private const string TextTooLongValidationMessage = "Text must be at most 2000 characters.";

    private const int TooLongTextLength = 2001;
    private const int WrittenHoursAgo = 1;

    private readonly IFixture _fixture;

    // Reviews are keyed on their shop, so a fresh shop per scenario keeps every other scenario's rows out.
    private readonly ShopEntity _shop;
    private readonly ShopEntity _otherShop;
    private readonly Guid _unknownReviewId = Guid.NewGuid();

    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _callerUsername = $"user-{Guid.NewGuid()}";
    private readonly string _otherUserId = Guid.NewGuid().ToString();

    private readonly UpdateReviewRequest _request;
    private readonly UpdateReviewRequest _textlessRequest;
    private readonly UpdateReviewRequest _tooLongRequest;

    private ReviewEntity _review;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId, _callerUsername));

    public Update_Shop_Review_Feature()
    {
        _fixture = new Fixture();

        var niche = Random.Shared.Niche();

        _shop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: niche);

        _otherShop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: niche);

        // Written a while ago, so an edit's updated_at lands clearly after created_at.
        var writtenAt = DateTime.UtcNow.AddHours(-WrittenHoursAgo);

        _review = _fixture.Build<ReviewEntity>()
            .With(review => review.ShopId, _shop.Id)
            .With(review => review.Niche, niche)
            .With(review => review.UpdateCount, 0)
            .With(review => review.CreatedAt, writtenAt)
            .With(review => review.UpdatedAt, writtenAt)
            .Create();

        _request = new UpdateReviewRequest { Text = _fixture.Create<string>() };
        _textlessRequest = _request with { Text = string.Empty };
        _tooLongRequest = _request with { Text = new string('a', TooLongTextLength) };
    }

    private async Task The_Review_Exists(string authorId)
    {
        _review = _review with { UserId = authorId };

        await ShopPersistenceProvider.Insert([_shop, _otherShop]);
        await ReviewsPersistenceProvider.Insert([_review]);
    }

    private async Task UpdateShopReview_Is_Called(Guid shopId, Guid reviewId, UpdateReviewRequest request)
    {
        _endpointLog = $"Updating review {reviewId} for shop {shopId}";
        _scopeValues = new Dictionary<string, object>
        {
            ["ShopId"] = shopId,
            ["ReviewId"] = reviewId,
            ["UserId"] = _callerId
        };

        _response = await Client.UpdateShopReview(shopId, reviewId, request);
    }

    private async Task The_Updated_Review_Is_Returned_And_Stored()
    {
        var stored = (await ReviewsPersistenceProvider.GetByShopId(_shop.Id)).Should().ContainSingle().Subject;
        var updated = await _response.Content.ReadFromJsonAsync<ReviewResponse>();

        updated.Should().BeEquivalentTo(new ReviewResponse
        {
            Id = _review.Id,
            ShopId = _shop.Id,
            Niche = _review.Niche,
            UserId = _callerId,
            Username = _review.Username,
            Text = _request.Text,
            CreatedAt = _review.CreatedAt,
            UpdatedAt = stored.UpdatedAt,
        }, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());

        stored.Body.Should().Be(_request.Text);
        stored.UpdateCount.Should().Be(1);
        stored.UpdatedAt.Should().BeAfter(_review.CreatedAt);
    }

    private async Task The_Stored_Review_Is_Unchanged()
    {
        var stored = await ReviewsPersistenceProvider.GetByShopId(_shop.Id);

        stored.Should().ContainSingle().Which.Should().BeEquivalentTo(_review, options => options
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1)))
            .WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Is_A_Not_Found_Problem(Guid shopId, Guid reviewId)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();
        var message = $"Review {reviewId} not found";

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = message,
            Detail = message,
            Instance = $"/shops/{shopId}/reviews/{reviewId}"
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
