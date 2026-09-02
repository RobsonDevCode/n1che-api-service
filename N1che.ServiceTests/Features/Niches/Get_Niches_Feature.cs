using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Niches;

public partial class Get_Niches_Feature
{
    [Scenario]
    public async Task Get_Niches_Returns_All_Seeded_Niches()
    {
        await Runner.RunScenarioAsync(
            when => GetNiches_Is_Called(),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => All_Seeded_Niches_Are_Returned(),
            and => Logs.There_Should_Be_A_Log(EndpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }
}
