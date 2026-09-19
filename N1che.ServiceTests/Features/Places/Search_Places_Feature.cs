using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Places;

public partial class Search_Places_Feature
{
    [Scenario]
    public async Task Search_Places_Returns_Places_When_Places_Match_In_The_Search_Area()
    {
        await Runner.RunScenarioAsync(
            given => Google_Returns_Successfully(_photographedPlace, _unphotographedPlace),
            when => SearchPlaces_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Places_Are_Returned(_photographedPlace, _unphotographedPlace),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Search_Places_Returns_An_Empty_Array_When_No_Places_Match()
    {
        await Runner.RunScenarioAsync(
            given => Google_Returns_Successfully(),
            when => SearchPlaces_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Places_Are_Returned(),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Search_Places_Returns_Only_The_Complete_Places_When_A_Place_Has_No_Address()
    {
        await Runner.RunScenarioAsync(
            given => Google_Returns_Successfully(_photographedPlace, _addresslessPlace),
            when => SearchPlaces_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Places_Are_Returned(_photographedPlace));
    }

    [Scenario]
    public async Task Search_Places_Returns_Bad_Gateway_When_Google_Cannot_Answer()
    {
        await Runner.RunScenarioAsync(
            given => Google_Returns_Unsuccessfully(),
            when => SearchPlaces_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadGateway),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadGateway, GoogleUnavailableMessage),
            and => Logs.There_Should_Be_A_Log(_googleFailureLog, LogLevel.Error, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_No_Query_Is_Supplied()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_queryLessFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(QueryField, QueryRequiredMessage));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_The_Query_Is_Blank()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_blankQueryFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(QueryField, QueryRequiredMessage));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_The_Corners_Share_A_Latitude()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_flatAreaFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NeLatField, CornerNotNorthMessage));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_The_Corners_Share_A_Longitude()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_narrowAreaFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NeLngField, CornerNotEastMessage));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_The_Area_Is_Too_Tall()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_tallAreaFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NeLatField, TooTallMessage));
    }

    [Scenario]
    public async Task Search_Places_Returns_A_Validation_Error_When_The_Area_Is_Too_Wide()
    {
        await Runner.RunScenarioAsync(
            when => SearchPlaces_Is_Called(_wideAreaFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NeLngField, TooWideMessage));
    }
}
