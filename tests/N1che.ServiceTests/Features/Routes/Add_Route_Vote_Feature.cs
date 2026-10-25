using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Add_Route_Vote_Feature
{
    [Scenario]
    public async Task Add_Route_Vote_Returns_No_Content_When_The_Caller_Has_Not_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            when => AddRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Votes_Are(_callerId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount + 1),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Route_Vote_Returns_No_Content_When_The_Caller_Is_The_Only_Voter()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            when => AddRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Votes_Are(_callerId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount + 1),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Route_Vote_Returns_Conflict_When_The_Callers_Vote_Is_Already_Stored()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_callerId),
            when => AddRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Conflict),
            and => The_Response_Is_A_Problem(HttpStatusCode.Conflict, _duplicateVoteMessage),
            and => The_Stored_Votes_Are(_callerId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Route_Vote_Returns_No_Content_When_Another_User_Has_Already_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => The_Route_Is_Voted_For_By(_otherUserId),
            when => AddRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Votes_Are(_otherUserId, _callerId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount + 1),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Route_Vote_Returns_Conflict_When_The_Caller_Has_Already_Voted()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => AddRouteVote_Is_Called(_route.Id),
            when => AddRouteVote_Is_Called(_route.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Conflict),
            and => The_Response_Is_A_Problem(HttpStatusCode.Conflict, _duplicateVoteMessage),
            and => The_Stored_Votes_Are(_callerId),
            and => The_Routes_Vote_Count_Is(_route.VoteCount + 1),
            and => Logs.There_Should_Be_A_Log(_duplicateVoteMessage, LogLevel.Warning, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Add_Route_Vote_Returns_Not_Found_When_No_Route_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => AddRouteVote_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _routeNotFoundMessage),
            and => No_Vote_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }
}
