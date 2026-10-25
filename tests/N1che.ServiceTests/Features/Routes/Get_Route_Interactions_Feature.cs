using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Route_Interactions_Feature
{
    [Scenario]
    public async Task Get_Route_Interactions_Returns_The_Callers_Own_Vote_State_When_The_Caller_Has_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_callerId),
            when => GetRouteInteractions_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: true),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_Interactions_Returns_Not_Voted_When_Only_Another_User_Has_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_otherUserId),
            when => GetRouteInteractions_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: false),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_Interactions_Returns_The_Live_Vote_Count_When_It_Changes_Between_Calls()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => GetRouteInteractions_Is_Called(_route.Id),
            when => The_Routes_Vote_Count_Changes(),
            and => GetRouteInteractions_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: false),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_Interactions_Returns_Not_Found_When_No_Route_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => GetRouteInteractions_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_Route_Not_Found(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_notFoundLog, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }
}
