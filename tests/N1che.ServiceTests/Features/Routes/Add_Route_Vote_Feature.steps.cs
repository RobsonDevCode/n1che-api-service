using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Add_Route_Vote_Feature : FeatureFixture
{
    private const string SuccessLog = "Route vote added";

    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The caller and the route are unique to this fixture, so scenarios never see each other's rows.
    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _otherUserId = Guid.NewGuid().ToString();
    private readonly ShopEntity _stop;
    private readonly RouteEntity _route;

    private Guid _requestedId;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = [];
    private string _endpointLog = string.Empty;
    private string _duplicateVoteMessage = string.Empty;
    private string _routeNotFoundMessage = string.Empty;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId));

    public Add_Route_Vote_Feature()
    {
        _fixture = new Fixture();

        _stop = ShopEntityBuilder.Build(_fixture,
            latitude: Random.Shared.Latitude(),
            longitude: Random.Shared.Longitude(),
            niches: NicheConstants.Goth);

        _route = RouteEntityBuilder.Build(_fixture, [_stop], NicheConstants.Goth,
            voteCount: Random.Shared.Next(1, 500));
    }

    private async Task The_Route_Exists()
    {
        await ShopPersistenceProvider.Insert([_stop]);
        await RoutePersistenceProvider.Upsert(_route, [_stop]);
    }

    private async Task The_Route_Is_Voted_For_By(string userId)
    {
        await RouteVotesPersistenceProvider.Insert(_route.Id, userId);
    }

    private async Task AddRouteVote_Is_Called(Guid id)
    {
        _requestedId = id;
        _scopeValues = new Dictionary<string, object> { ["RouteId"] = id, ["UserId"] = _callerId };
        _endpointLog = $"Adding vote for route {id}";
        _duplicateVoteMessage = $"Vote for route {id} already exists.";
        _routeNotFoundMessage = $"Route {id} not found";

        _response = await Client.AddRouteVote(id);
    }

    private async Task The_Stored_Votes_Are(params string[] userIds)
    {
        var votes = await RouteVotesPersistenceProvider.GetByRouteId(_route.Id);

        votes.Should().BeEquivalentTo(
            userIds.Select(userId => new { RouteId = _route.Id, UserId = userId }),
            options => options.ExcludingMissingMembers());
    }

    private async Task The_Routes_Vote_Count_Is(int expected)
    {
        var route = await RoutePersistenceProvider.GetById(_route.Id);

        route.VoteCount.Should().Be(expected);
    }

    private async Task No_Vote_Is_Stored()
    {
        var votes = await RouteVotesPersistenceProvider.GetByRouteId(_requestedId);

        votes.Should().BeEmpty();
    }

    private async Task The_Response_Is_A_Problem(HttpStatusCode statusCode, string message)
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Detail = message,
            Instance = $"/routes/{_requestedId}/vote"
        }, options => options.Excluding(details => details.Extensions));
    }
}
