using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Compute_Route_Feature
{
    [Scenario]
    public async Task Compute_Route_Returns_The_Walk_When_Google_Routes_Through_The_Stops()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(3),
            and => Google_Returns_A_Walk_Through(StopAt(0), StopAt(1), StopAt(2)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Walk_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_The_Walk_From_The_Origin_When_An_Origin_Is_Supplied()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(2),
            and => Google_Returns_A_Walk_Through(_origin, StopAt(0), StopAt(1)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode, fromOrigin: true)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Walk_Is_Returned(fromOrigin: true),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_The_Walk_Back_To_The_First_Stop_When_The_Mode_Is_Loop()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(3),
            and => Google_Returns_A_Walk_Through(StopAt(0), StopAt(1), StopAt(2), StopAt(0)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(LoopMode)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Walk_Is_Returned(fromOrigin: false, isLoop: true),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_The_Walk_To_A_Single_Stop_When_An_Origin_Is_Supplied()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(1),
            and => Google_Returns_A_Walk_Through(_origin, StopAt(0)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode, fromOrigin: true)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Walk_Is_Returned(fromOrigin: true));
    }

    [Scenario]
    public async Task Compute_Route_Returns_An_Unknown_Maneuver_When_A_Step_Carries_No_Instruction()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(2),
            and => Google_Returns_A_Walk_Whose_Step_Has_No_Instruction(StopAt(0), StopAt(1)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Walk_Is_Returned());
    }

    [Scenario]
    public async Task Compute_Route_Returns_Bad_Request_When_Google_Routes_Nothing_Through_The_Stops()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(2),
            and => Google_Routes_Nothing_Through(StopAt(0), StopAt(1)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, NoWalkMessage),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_Bad_Gateway_When_Google_Cannot_Answer()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(2),
            and => Google_Answers_Unsuccessfully(StopAt(0), StopAt(1)),
            when => ComputeRoute_Is_Called(WalkThroughTheStops(YouMode)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadGateway),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadGateway, GoogleUnavailableMessage),
            and => Logs.There_Should_Be_A_Log(_googleFailureLog, LogLevel.Error, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_Not_Found_When_A_Stop_Names_No_Shop()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_unknownStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _notFoundMessage),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_The_Mode_Is_Not_Recognised()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_unrecognisedModeRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(ModeField, ModeValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_No_Stops_Are_Supplied()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_stopLessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, StopRequiredValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_More_Stops_Are_Supplied_Than_A_Route_Can_Hold()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_tooManyStopsRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, TooManyStopsValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_A_Stop_Appears_Twice()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_repeatedStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, RepeatedStopValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_A_Loop_Has_A_Single_Stop()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_singleStopLoopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, LoopStopsValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_A_Single_Stop_Has_No_Origin_To_Walk_From()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_originlessSingleStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, OriginlessStopsValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_The_Origin_Latitude_Is_Out_Of_Range()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_outOfRangeLatitudeRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(OriginLatitudeField, LatitudeValidationMessage));
    }

    [Scenario]
    public async Task Compute_Route_Returns_A_Validation_Error_When_The_Origin_Longitude_Is_Out_Of_Range()
    {
        await Runner.RunScenarioAsync(
            when => ComputeRoute_Is_Called(_outOfRangeLongitudeRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(OriginLongitudeField, LongitudeValidationMessage));
    }
}
