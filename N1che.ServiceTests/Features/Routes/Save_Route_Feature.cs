using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Save_Route_Feature
{
    [Scenario]
    public async Task Save_Route_Returns_The_Created_Route()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(3),
            when => CreateRoute_Is_Called(SaveTheStops(_niche)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Route_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Save_Route_Stores_The_Walk_The_Caller_Sent_In_The_Order_The_Stops_Are_Walked()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(3),
            when => CreateRoute_Is_Called(SaveTheStops(_niche)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Stored_Route_Is_The_Walk_The_Caller_Sent(),
            and => The_Stored_Stops_Are_In_Walk_Order());
    }

    [Scenario]
    public async Task Save_Route_Returns_Bad_Request_When_The_Niche_Is_Not_Recognised()
    {
        await Runner.RunScenarioAsync(
            given => Shops_Exist(2),
            when => CreateRoute_Is_Called(SaveTheStops(UnrecognisedNiche)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, UnrecognisedNicheMessage),
            and => No_Route_Is_Stored(),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Save_Route_Returns_Not_Found_When_A_Stop_Names_No_Shop()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_unknownStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _notFoundMessage),
            and => No_Route_Is_Stored(),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_A_Single_Stop_Is_Saved()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_singleStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, TooFewStopsValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_More_Stops_Are_Saved_Than_A_Route_Can_Hold()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_tooManyStopsRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, TooManyStopsValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_A_Stop_Appears_Twice()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_repeatedStopRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(StopsField, RepeatedStopValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_The_Polyline_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_polylineLessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(PolylineField, PolylineValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_The_Name_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_namelessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NameField, NameValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_The_Tag_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_taglessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(TagField, TagValidationMessage));
    }

    [Scenario]
    public async Task Save_Route_Returns_A_Validation_Error_When_The_Niche_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateRoute_Is_Called(_nicheLessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NicheField, NicheValidationMessage));
    }
}
