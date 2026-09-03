using LightBDD.XUnit2;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;

namespace N1che.ServiceTests.Features.Auth;

public partial class Endpoints_Require_Authentication_Feature : FeatureFixture
{
    private const string InvalidToken = "not-a-valid-jwt";

    private HttpResponseMessage _response = null!;

    private async Task The_Endpoint_Is_Called_Without_A_Token()
    {
        _response = await TestWebApplicationFactory.Instance.CreateClient().GetNiches();
    }

    private async Task The_Endpoint_Is_Called_With_An_Invalid_Token()
    {
        _response = await TestWebApplicationFactory.Instance.CreateAuthenticatedClient(InvalidToken).GetNiches();
    }
}
