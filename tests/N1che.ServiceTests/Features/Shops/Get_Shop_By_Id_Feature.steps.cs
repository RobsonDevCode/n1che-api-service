using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Response.Shops;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Shop_By_Id_Feature : FeatureFixture
{
    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // Built in the constructor so every scenario knows the id it is going to ask for.
    private readonly ShopEntity _shop;

    private ShopHoursEntity? _hours;
    private Guid _requestedId;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = null!;
    private string _endpointLog = null!;
    private string _notFoundLog = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    private const string SuccessLog = "Shop retrieved";

    public Get_Shop_By_Id_Feature()
    {
        _fixture = new Fixture();

        // Coordinates stay in range so the geography cast doesn't silently coerce them,
        // and CreatedAt is UTC so it survives the timestamptz round trip unchanged.
        _shop = _fixture.Build<ShopEntity>()
            .With(shop => shop.Latitude, Random.Shared.Next(-80, 80) + Random.Shared.NextDouble())
            .With(shop => shop.Longitude, Random.Shared.Next(-170, 170) + Random.Shared.NextDouble())
            .With(shop => shop.CreatedAt, DateTime.UtcNow)
            .Create();
    }

    private async Task The_Shop_Exists()
    {
        await ShopPersistenceProvider.Insert([_shop]);
    }

    private async Task The_Shop_Trades_Today()
    {
        // Whole-minute hours, so the value survives a round trip through a Postgres `time` column.
        _hours = _fixture.Build<ShopHoursEntity>()
            .With(hours => hours.ShopId, _shop.Id)
            .With(hours => hours.DayOfWeek, (int)DateTime.UtcNow.DayOfWeek)
            .With(hours => hours.OpenTime, new TimeOnly(Random.Shared.Next(6, 12), 0))
            .With(hours => hours.CloseTime, new TimeOnly(Random.Shared.Next(17, 23), 30))
            .Create();

        await ShopPersistenceProvider.InsertHours([_hours]);
    }

    private async Task The_Shop_Is_Renamed_In_The_Database()
    {
        await ShopPersistenceProvider.Update(_shop with { Name = _fixture.Create<string>() });
    }

    private async Task GetShopById_Is_Called(Guid id)
    {
        _requestedId = id;
        _scopeValues = new Dictionary<string, object> { ["ShopId"] = id };
        _endpointLog = $"Getting shop {id}";
        _notFoundLog = $"Shop {id} not found";

        _response = await Client.GetShopById(id);
    }

    private async Task The_Shop_Is_Returned()
    {
        var shop = await _response.Content.ReadFromJsonAsync<ShopDetailResponse>();

        shop.Should().BeEquivalentTo(new ShopDetailResponse
        {
            Id = _shop.Id,
            GooglePlaceId = _shop.GooglePlaceId,
            Name = _shop.Name,
            Niches = _shop.Niches,
            Address = _shop.Address,
            Latitude = _shop.Latitude,
            Longitude = _shop.Longitude,
            PlaceStatus = _shop.PlaceStatus,
            OpenTime = _hours?.OpenTime,
            CloseTime = _hours?.CloseTime,
            CreatedAt = _shop.CreatedAt,
            AddedByUserId = _shop.AddedByUserId,
            AddedByUsername = _shop.AddedByUsername,
            PhotoUrl = null
        }, options => options
            .Using<double>(ctx => ctx.Subject.Should().BeApproximately(ctx.Expectation, 1e-6)).WhenTypeIs<double>()
            .Using<DateTime>(ctx => ctx.Subject.Should().BeCloseTo(ctx.Expectation, TimeSpan.FromSeconds(1))).WhenTypeIs<DateTime>());
    }

    private async Task The_Response_Is_Shop_Not_Found()
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = $"Shop {_requestedId} not found",
            Detail = $"Shop {_requestedId} not found",
            Instance = $"/shops/{_requestedId}"
        }, options => options.Excluding(details => details.Extensions));
    }
}
