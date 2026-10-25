using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using LightBDD.XUnit2;
using Microsoft.AspNetCore.Mvc;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;

namespace N1che.ServiceTests.Features.Auth;

public partial class Endpoints_Require_Authentication_Feature : FeatureFixture
{
    private const string InvalidToken = "not-a-valid-jwt";
    private const string UnauthorizedMessage = "Unauthorized";

    // The endpoint's scope is disposed as the exception unwinds, so the handler's warning carries none.
    private static readonly Dictionary<string, object> NoScopes = [];

    private readonly Guid _shopId = Guid.NewGuid();

    private HttpResponseMessage _response = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;

    private async Task The_Endpoint_Is_Called_Without_A_Token()
    {
        _response = await TestWebApplicationFactory.Instance.CreateClient().GetNiches();
    }

    private async Task The_Endpoint_Is_Called_With_An_Invalid_Token()
    {
        _response = await TestWebApplicationFactory.Instance.CreateAuthenticatedClient(InvalidToken).GetNiches();
    }

    private async Task A_Per_User_Endpoint_Is_Called_With_A_Token_Missing_The_Subject_Claim()
    {
        _response = await CallPerUserEndpoint(TestAuth.GenerateToken(sub: null));
    }

    private async Task A_Per_User_Endpoint_Is_Called_With_A_Token_Missing_The_Username_Claim()
    {
        _response = await CallPerUserEndpoint(TestAuth.GenerateToken(username: null));
    }

    private async Task<HttpResponseMessage> CallPerUserEndpoint(string token) =>
        await TestWebApplicationFactory.Instance.CreateAuthenticatedClient(token).GetShopInteractions(_shopId);

    private async Task The_Response_Is_Unauthorized()
    {
        var problem = await _response.Content.ReadFromJsonAsync<ProblemDetails>();

        problem.Should().BeEquivalentTo(new ProblemDetails
        {
            Status = (int)HttpStatusCode.Unauthorized,
            Title = UnauthorizedMessage,
            Detail = UnauthorizedMessage,
            Instance = $"/shops/{_shopId}/interactions"
        }, options => options.Excluding(details => details.Extensions));
    }
}
