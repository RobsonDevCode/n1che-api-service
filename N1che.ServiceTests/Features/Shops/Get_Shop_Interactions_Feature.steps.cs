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

public partial class Get_Shop_Interactions_Feature : FeatureFixture
{
    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The caller and the shop are unique to this fixture, so scenarios never see each other's rows.
    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _otherUserId = Guid.NewGuid().ToString();
    private ShopEntity _shop;

    private Guid _requestedId;
    private HttpResponseMessage _response;
    private Dictionary<string, object> _scopeValues = [];
    private string _endpointLog = string.Empty;
    private string _notFoundLog = string.Empty;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId));

    private const string SuccessLog = "Shop interactions retrieved";

    public Get_Shop_Interactions_Feature()
    {
        _fixture = new Fixture();

        // Coordinates stay in range so the geography cast doesn't silently coerce them.
        _shop = _fixture.Build<ShopEntity>()
            .With(shop => shop.Latitude, Random.Shared.Next(-80, 80) + Random.Shared.NextDouble())
            .With(shop => shop.Longitude, Random.Shared.Next(-170, 170) + Random.Shared.NextDouble())
            .With(shop => shop.VoteCount, Random.Shared.Next(1, 500))
            .With(shop => shop.CreatedAt, DateTime.UtcNow)
            .Create();
    }

    private async Task The_Shop_Exists()
    {
        await ShopPersistenceProvider.Insert([_shop]);
    }

    private async Task The_Shop_Is_Voted_For_By(string userId)
    {
        await ShopVotesPersistenceProvider.Insert(_shop.Id, userId);
    }

    private async Task The_Shop_Is_Bookmarked_By(string userId)
    {
        await BookmarksPersistenceProvider.Insert(_shop.Id, userId);
    }

    private async Task The_Shops_Vote_Count_Changes()
    {
        _shop = _shop with { VoteCount = _shop.VoteCount + Random.Shared.Next(1, 100) };

        await ShopPersistenceProvider.Update(_shop);
    }

    private async Task GetShopInteractions_Is_Called(Guid id)
    {
        _requestedId = id;
        _scopeValues = new Dictionary<string, object> { ["ShopId"] = id, ["UserId"] = _callerId };
        _endpointLog = $"Getting interactions for shop {id}";
        _notFoundLog = $"Shop {id} not found";

        _response = await Client.GetShopInteractions(id);
    }

    private async Task The_Interactions_Are_Returned(bool voted, bool saved)
    {
        var interactions = await _response.Content.ReadFromJsonAsync<ShopInteractionsResponse>();

        interactions.Should().BeEquivalentTo(new ShopInteractionsResponse
        {
            VoteCount = _shop.VoteCount,
            Voted = voted,
            Saved = saved
        });
    }

    private async Task The_Response_Is_Shop_Not_Found()
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = $"Shop {_requestedId} not found",
            Detail = $"Shop {_requestedId} not found",
            Instance = $"/shops/{_requestedId}/interactions"
        }, options => options.Excluding(details => details.Extensions));
    }
}
