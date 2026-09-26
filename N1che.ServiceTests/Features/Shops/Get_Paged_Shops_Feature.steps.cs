using System.Globalization;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Http;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Shops;
using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Paged_Shops_Feature : FeatureFixture
{
    private const int SeededShopCount = 5;
    private const int Page = 1;
    private const int Size = 2;

    // Location is irrelevant to the popularity query but must be a valid geography point to insert.
    private const double Latitude = 51.5;
    private const double Longitude = -0.1;

    private readonly IFixture _fixture;

    private List<ShopEntity> _shops;
    private Dictionary<Guid, ShopHoursEntity> _hours = [];
    private ShopsFilter _filter;
    private PaginationFilter _pagination;
    private HttpResponseMessage _response;
    private Dictionary<string, object> _scopeValues;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    private readonly string EndpointLog;
    private const string SuccessLog = "Shops page retrieved";

    public Get_Paged_Shops_Feature()
    {
        _fixture = new Fixture();

        EndpointLog = string.Format(CultureInfo.InvariantCulture,
            "Getting page {0} of shops (size {1}) for niche {2}", Page, Size, NicheConstants.Cottagecore);
    }

    // Seeded with the niche under test. The niche-filtered scenarios use a niche no other feature
    // seeds, so their query only ever sees this scenario's rows.
    private async Task Shops_Exist(string niche)
    {
        _shops = ShopEntityBuilder.BuildMany(_fixture, SeededShopCount,
            latitude: Latitude, longitude: Longitude, niches: niche);

        await ShopPersistenceProvider.Insert(_shops);

        // Every shop but the first trades today, so the left join's missing-hours path is covered too.
        _hours = _shops.Skip(1)
            .Select(shop => ShopHoursEntityBuilder.BuildForToday(_fixture, shop.Id))
            .ToDictionary(hours => hours.ShopId);

        await ShopPersistenceProvider.InsertHours(_hours.Values);
    }

    private async Task GetShopsPage_Is_Called(ShopsFilter filter, PaginationFilter pagination)
    {
        _filter = filter;
        _pagination = pagination;
        _scopeValues = new Dictionary<string, object>
        {
            ["Niche"] = string.Join(" | ", filter.Niche ?? ["all"]),
            ["Page"] = pagination.Page,
            ["Size"] = pagination.Size
        };

        _response = await Client.GetShopsPage(filter, pagination);
    }

    private async Task The_Expected_Page_Is_Returned()
    {
        var result = await _response.Content.ReadFromJsonAsync<PagedResponse<ShopResponse>>();
        result.Should().NotBeNull();

        var expected = _shops
            .OrderByDescending(shop => shop.VoteCount)
            .ThenByDescending(shop => shop.CreatedAt)
            .Skip((_pagination.Page - 1) * _pagination.Size)
            .Take(_pagination.Size)
            .Select(Expected)
            .ToArray();

        result!.PaginationDetails.CurrentPage.Should().Be(_pagination.Page);
        result.PaginationDetails.PageSize.Should().Be(_pagination.Size);
        result.PaginationDetails.TotalCount.Should().BeGreaterThanOrEqualTo(_shops.Count);

        // Filter to this scenario's own rows so shared-table rows from other scenarios never interfere.
        var seededIds = _shops.Select(shop => shop.Id).ToHashSet();
        var returned = result.Data.Where(shop => seededIds.Contains(shop.Id)).ToArray();

        returned.Should().BeEquivalentTo(expected, options => options
            .WithStrictOrdering()
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Returned_Page_Is_Ordered_By_Popularity()
    {
        var result = await _response.Content.ReadFromJsonAsync<PagedResponse<ShopResponse>>();
        result.Should().NotBeNull();

        result!.Data.Select(shop => shop.VoteCount).Should().BeInDescendingOrder();
        result.PaginationDetails.TotalCount.Should().BeGreaterThanOrEqualTo(_shops.Count);
    }

    private async Task The_Response_Contains_A_Validation_Error_For(string field)
    {
        var problem = await _response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey(field);
    }

    private ShopResponse Expected(ShopEntity shop)
    {
        var hours = _hours.GetValueOrDefault(shop.Id);

        return new ShopResponse
        {
            Id = shop.Id,
            GooglePlaceId = shop.GooglePlaceId,
            Name = shop.Name,
            Niches = shop.Niches,
            Address = shop.Address,
            Latitude = shop.Latitude,
            Longitude = shop.Longitude,
            VoteCount = shop.VoteCount,
            PlaceStatus = shop.PlaceStatus,
            OpenTime = hours?.OpenTime,
            CloseTime = hours?.CloseTime,
            CreatedAt = shop.CreatedAt,
            AddedByUserId = shop.AddedByUserId,
            AddedByUsername = shop.AddedByUsername,
            PhotoUrl = null
        };
    }
}
