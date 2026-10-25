using System.Globalization;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Reviews;
using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Reviews;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Shop_Reviews_Feature : FeatureFixture
{
    private const int Page = 1;
    private const int Size = 2;

    private readonly IFixture _fixture;

    // Reviews are keyed on their shop, so a fresh shop per scenario keeps every other scenario's rows out.
    private readonly ShopEntity _shop;
    private readonly ShopEntity _otherShop;

    private List<ReviewEntity> _reviews = [];
    private ReviewsFilter _filter = null!;
    private PaginationFilter _pagination = null!;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    private readonly string EndpointLog;
    private const string SuccessLog = "Shop reviews page retrieved";

    public Get_Shop_Reviews_Feature()
    {
        _fixture = new Fixture();

        _shop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: [NicheConstants.Goth, NicheConstants.Vintage, NicheConstants.Skater]);

        _otherShop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: NicheConstants.Goth);

        EndpointLog = string.Format(CultureInfo.InvariantCulture,
            "Getting page {0} of reviews (size {1}) for shop {2} in niche {3}", Page, Size, _shop.Id, NicheConstants.Goth);
    }

    private async Task Reviews_Exist()
    {
        await ShopPersistenceProvider.Insert([_shop, _otherShop]);

        // Distinct timestamps make the newest-first order deterministic.
        _reviews =
        [
            ReviewOf(_shop, NicheConstants.Goth, minutesAgo: 5),
            ReviewOf(_shop, NicheConstants.Goth, minutesAgo: 1),
            ReviewOf(_shop, NicheConstants.Goth, minutesAgo: 30),
            ReviewOf(_shop, NicheConstants.Goth, minutesAgo: 10),
            ReviewOf(_shop, NicheConstants.Vintage, minutesAgo: 2),
            ReviewOf(_otherShop, NicheConstants.Goth, minutesAgo: 3)
        ];

        await ReviewsPersistenceProvider.Insert(_reviews);
    }

    private async Task GetShopReviews_Is_Called(ReviewsFilter filter, PaginationFilter pagination)
    {
        _filter = filter;
        _pagination = pagination;
        _scopeValues = new Dictionary<string, object>
        {
            ["ShopId"] = _shop.Id,
            ["Niche"] = filter.Niche,
            ["Page"] = pagination.Page,
            ["Size"] = pagination.Size
        };

        _response = await Client.GetShopReviews(_shop.Id, filter, pagination);
    }

    private async Task The_Expected_Page_Is_Returned()
    {
        var result = await _response.Content.ReadFromJsonAsync<PagedResponse<ReviewResponse>>();

        var matching = _reviews
            .Where(review => review.ShopId == _shop.Id && review.Niche == _filter.Niche)
            .OrderByDescending(review => review.CreatedAt)
            .ToArray();

        result.Should().BeEquivalentTo(new PagedResponse<ReviewResponse>
        {
            Data = matching
                .Skip((_pagination.Page - 1) * _pagination.Size)
                .Take(_pagination.Size)
                .Select(review => new ReviewResponse
                {
                    Id = review.Id,
                    ShopId = review.ShopId,
                    Niche = review.Niche,
                    UserId = review.UserId,
                    Username = review.Username,
                    Text = review.Body,
                    CreatedAt = review.CreatedAt,
                    UpdatedAt = review.UpdatedAt
                }),
            PaginationDetails = new PaginationDetailsResponse
            {
                TotalCount = matching.Length,
                CurrentPage = _pagination.Page,
                PageSize = _pagination.Size
            }
        }, options => options
            .WithStrictOrdering()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Contains_A_Validation_Error_For(string field)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem.Errors.Should().ContainKey(field);
    }

    private ReviewEntity ReviewOf(ShopEntity shop, string niche, int minutesAgo)
    {
        var createdAt = DateTime.UtcNow.AddMinutes(-minutesAgo);

        return _fixture.Build<ReviewEntity>()
            .With(review => review.ShopId, shop.Id)
            .With(review => review.Niche, niche)
            .With(review => review.CreatedAt, createdAt)
            .With(review => review.UpdatedAt, createdAt)
            .Create();
    }
}
