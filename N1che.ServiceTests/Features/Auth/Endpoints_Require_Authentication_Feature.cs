using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Auth;

public partial class Endpoints_Require_Authentication_Feature
{
    [Scenario]
    public async Task Request_Without_A_Token_Returns_Unauthorized()
    {
        await Runner.RunScenarioAsync(
            when => The_Endpoint_Is_Called_Without_A_Token(),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Unauthorized));
    }

    [Scenario]
    public async Task Request_With_An_Invalid_Token_Returns_Unauthorized()
    {
        await Runner.RunScenarioAsync(
            when => The_Endpoint_Is_Called_With_An_Invalid_Token(),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Unauthorized));
    }
}
