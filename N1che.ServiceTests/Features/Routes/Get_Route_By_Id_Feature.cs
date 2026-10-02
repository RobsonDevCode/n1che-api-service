using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Route_By_Id_Feature
{
    [Scenario]
    public async Task Get_Route_By_Id_Returns_The_Walk_From_The_Origin_When_The_First_Stop_Is_Within_Reach()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            and => Google_Returns_A_Walk_From_The_Origin_Through_The_Stops(),
            when => GetRouteById_Is_Called(_route.Id, _nearbyOrigin),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Route_Is_Returned_With_The_Walk(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_By_Id_Returns_Not_Found_When_No_Route_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => GetRouteById_Is_Called(_unknownRouteId, _nearbyOrigin),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _notFoundMessage),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_notFoundMessage, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_By_Id_Returns_Unprocessable_Entity_When_The_First_Stop_Is_Too_Far_From_The_Origin()
    {
        await Runner.RunScenarioAsync(
            given => The_Route_Exists(),
            when => GetRouteById_Is_Called(_route.Id, _distantOrigin),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.UnprocessableEntity),
            and => The_Response_Is_A_Problem(HttpStatusCode.UnprocessableEntity, _tooFarMessage),
            and => Logs.There_Should_Be_A_Log(_tooFarMessage, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Route_By_Id_Returns_A_Validation_Error_When_The_Latitude_Is_Out_Of_Range()
    {
        await Runner.RunScenarioAsync(
            when => GetRouteById_Is_Called(_unknownRouteId, _outOfRangeLatitudeOrigin),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(LatField, LatitudeValidationMessage));
    }

    [Scenario]
    public async Task Get_Route_By_Id_Returns_A_Validation_Error_When_The_Longitude_Is_Out_Of_Range()
    {
        await Runner.RunScenarioAsync(
            when => GetRouteById_Is_Called(_unknownRouteId, _outOfRangeLongitudeOrigin),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(LngField, LongitudeValidationMessage));
    }
}
