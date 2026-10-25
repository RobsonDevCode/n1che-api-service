using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc;
using N1che.Contracts.Response.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Routes;
using N1che.Persistence.Postgres.Postgres.Entities.Shops;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;
using N1che.ServiceTests.Infrastructure.Persistence;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Route_Interactions_Feature : FeatureFixture
{
    private const string SuccessLog = "Route interactions retrieved";

    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly IFixture _fixture;

    // The caller and the route are unique to this fixture, so scenarios never see each other's rows.
    private readonly string _callerId = Guid.NewGuid().ToString();
    private readonly string _otherUserId = Guid.NewGuid().ToString();
    private readonly ShopEntity _stop;
    private RouteEntity _route;

    private Guid _requestedId;
    private HttpResponseMessage _response = null!;
    private Dictionary<string, object> _scopeValues = [];
    private string _endpointLog = string.Empty;
    private string _notFoundLog = string.Empty;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private HttpClient Client => TestWebApplicationFactory.Instance
        .CreateAuthenticatedClient(TestAuth.GenerateToken(_callerId));

    public Get_Route_Interactions_Feature()
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

    private async Task The_Routes_Vote_Count_Changes()
    {
        _route = _route with { VoteCount = _route.VoteCount + Random.Shared.Next(1, 100) };

        await RoutePersistenceProvider.Upsert(_route, [_stop]);
    }

    private async Task GetRouteInteractions_Is_Called(Guid id)
    {
        _requestedId = id;
        _scopeValues = new Dictionary<string, object> { ["RouteId"] = id, ["UserId"] = _callerId };
        _endpointLog = $"Getting interactions for route {id}";
        _notFoundLog = $"Route {id} not found";

        _response = await Client.GetRouteInteractions(id);
    }

    private async Task The_Interactions_Are_Returned(bool voted)
    {
        var interactions = await _response.Content.ReadFromJsonAsync<RouteInteractionsResponse>();

        interactions.Should().BeEquivalentTo(new RouteInteractionsResponse
        {
            VoteCount = _route.VoteCount,
            Voted = voted
        });
    }

    private async Task The_Response_Is_Route_Not_Found()
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)HttpStatusCode.NotFound,
            Title = $"Route {_requestedId} not found",
            Detail = $"Route {_requestedId} not found",
            Instance = $"/routes/{_requestedId}/interactions"
        }, options => options.Excluding(details => details.Extensions));
    }
}
