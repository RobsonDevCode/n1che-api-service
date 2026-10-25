using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Shops;

public partial class Remove_Shop_Vote_Feature : FeatureFixture
{
    private const string SuccessLog = "Shop vote removed";

    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The caller and the shop are unique to this fixture, so scenarios never see each other's rows.
    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _otherUserId = Guid.NewGuid().ToString();
    private readonly ShopEntity _shop;

    private Guid _requestedId;
    private HttpResponseMessage _response;
    private Dictionary<string, object> _scopeValues = [];
    private string _endpointLog = string.Empty;
    private string _voteNotFoundMessage = string.Empty;
    private string _shopNotFoundMessage = string.Empty;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId));

    public Remove_Shop_Vote_Feature()
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

    private async Task RemoveShopVote_Is_Called(Guid id)
    {
        _requestedId = id;
        _scopeValues = new Dictionary<string, object> { ["ShopId"] = id, ["UserId"] = _callerId };
        _endpointLog = $"Removing vote for shop {id}";
        _voteNotFoundMessage = $"Vote for shop {id} not found";
        _shopNotFoundMessage = $"Shop {id} not found";

        _response = await Client.RemoveShopVote(id);
    }

    private async Task The_Stored_Votes_Are(params string[] userIds)
    {
        var votes = await ShopVotesPersistenceProvider.GetByShopId(_shop.Id);

        votes.Should().BeEquivalentTo(
            userIds.Select(userId => new { ShopId = _shop.Id, UserId = userId }),
            options => options.ExcludingMissingMembers());
    }

    private async Task The_Shops_Vote_Count_Is(int expected)
    {
        var shop = await ShopPersistenceProvider.GetById(_shop.Id);

        shop.VoteCount.Should().Be(expected);
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = $"/shops/{_requestedId}/vote"
        }, options => options.Excluding(details => details.Extensions));
    }
}
