using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Remove_Route_Vote_Feature
{
    [Scenario]
    public async Task Remove_Route_Vote_Returns_No_Content_When_The_Caller_Has_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_callerId),
            when => RemoveRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Votes_Are(),
            and => The_Routes_Vote_Count_Is(_route.VoteCount - 1),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Route_Vote_Returns_No_Content_When_Another_User_Has_Also_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_otherUserId),
            and => The_Route_Is_Voted_For_By(_callerId),
            when => RemoveRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Votes_Are(_otherUserId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount - 1),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Route_Vote_Returns_Not_Found_When_The_Caller_Has_Not_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_otherUserId),
            when => RemoveRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _voteNotFoundMessage),
            and => The_Stored_Votes_Are(_otherUserId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount),
            and => Logs.There_Should_Be_A_Log(_voteNotFoundMessage, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Route_Vote_Returns_Not_Found_When_No_Route_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => RemoveRouteVote_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _routeNotFoundMessage),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_routeNotFoundMessage, LogLevel.Warning, NoScopes, TestLogger));
    }
}
