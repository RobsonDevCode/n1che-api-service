using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Places;

public partial class Get_Place_Photo_Media_Feature
{
    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_The_Photo_Uri_When_Google_Holds_The_Photo()
    {
        await Runner.RunScenarioAsync(
            given => Google_Returns_The_Photo_Media(),
            when => GetPlacePhotoMedia_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Photo_Uri_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_Not_Found_When_Google_Holds_No_Such_Photo()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _notFoundMessage),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_notFoundMessage, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_Bad_Request_When_Google_Rejects_The_Photo_Reference()
    {
        await Runner.RunScenarioAsync(
            given => Google_Answers_With(HttpStatusCode.BadRequest),
            when => GetPlacePhotoMedia_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, _rejectedMessage),
            and => Logs.There_Should_Be_A_Log(_googleRejectedLog, LogLevel.Error, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_Bad_Gateway_When_Google_Cannot_Answer()
    {
        await Runner.RunScenarioAsync(
            given => Google_Answers_With(RejectedApiKeyStatus),
            when => GetPlacePhotoMedia_Is_Called(_filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadGateway),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadGateway, GoogleUnavailableMessage),
            and => Logs.There_Should_Be_A_Log(_googleFailureLog, LogLevel.Error, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_A_Validation_Error_When_The_Place_Id_Has_Invalid_Characters()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_invalidPlaceIdFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(PlaceIdField, PlaceIdInvalidMessage));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_A_Validation_Error_When_The_Place_Id_Ends_In_A_Newline()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_newlinePlaceIdFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(PlaceIdField, PlaceIdInvalidMessage));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_A_Validation_Error_When_The_Photo_Id_Has_Invalid_Characters()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_invalidPhotoIdFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(PhotoIdField, PhotoIdInvalidMessage));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_A_Validation_Error_When_The_Max_Width_Is_Zero()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_zeroWidthFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(MaxWidthField, MaxWidthOutOfRangeMessage));
    }

    [Scenario]
    public async Task Get_Place_Photo_Media_Returns_A_Validation_Error_When_The_Max_Width_Is_Above_Google_Maximum()
    {
        await Runner.RunScenarioAsync(
            when => GetPlacePhotoMedia_Is_Called(_oversizedWidthFilter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(MaxWidthField, MaxWidthOutOfRangeMessage));
    }
}
