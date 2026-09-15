using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
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

    [Scenario]
    public async Task Request_With_A_Token_Missing_The_Subject_Claim_Returns_Unauthorized()
    {
        await Runner.RunScenarioAsync(
            when => A_Per_User_Endpoint_Is_Called_With_A_Token_Missing_The_Subject_Claim(),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Unauthorized),
            and => The_Response_Is_Unauthorized(),
            and => Logs.There_Should_Be_A_Log(UnauthorizedMessage, LogLevel.Warning, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Request_With_A_Token_Missing_The_Username_Claim_Returns_Unauthorized()
    {
        await Runner.RunScenarioAsync(
            when => A_Per_User_Endpoint_Is_Called_With_A_Token_Missing_The_Username_Claim(),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Unauthorized),
            and => The_Response_Is_Unauthorized(),
            and => Logs.There_Should_Be_A_Log(UnauthorizedMessage, LogLevel.Warning, NoScopes, TestLogger));
    }
}
