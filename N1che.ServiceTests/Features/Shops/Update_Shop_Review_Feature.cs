using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Update_Shop_Review_Feature
{
    [Scenario]
    public async Task Update_Shop_Review_Returns_The_Updated_Review_When_The_Caller_Wrote_It()
    {
        await Runner.RunScenarioAsync(
            given => The_Review_Exists(_callerId),
            when => UpdateShopReview_Is_Called(_shop.Id, _review.Id, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Updated_Review_Is_Returned_And_Stored(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Update_Shop_Review_Returns_Not_Found_When_Another_User_Wrote_The_Review()
    {
        await Runner.RunScenarioAsync(
            given => The_Review_Exists(_otherUserId),
            when => UpdateShopReview_Is_Called(_shop.Id, _review.Id, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Not_Found_Problem(_shop.Id, _review.Id),
            and => The_Stored_Review_Is_Unchanged(),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Update_Shop_Review_Returns_Not_Found_When_The_Review_Belongs_To_Another_Shop()
    {
        await Runner.RunScenarioAsync(
            given => The_Review_Exists(_callerId),
            when => UpdateShopReview_Is_Called(_otherShop.Id, _review.Id, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Not_Found_Problem(_otherShop.Id, _review.Id),
            and => The_Stored_Review_Is_Unchanged(),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Update_Shop_Review_Returns_Not_Found_When_No_Review_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => UpdateShopReview_Is_Called(_shop.Id, _unknownReviewId, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Not_Found_Problem(_shop.Id, _unknownReviewId),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Update_Shop_Review_Returns_A_Validation_Error_When_The_Text_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => UpdateShopReview_Is_Called(_shop.Id, _unknownReviewId, _textlessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(TextField, TextValidationMessage));
    }

    [Scenario]
    public async Task Update_Shop_Review_Returns_A_Validation_Error_When_The_Text_Is_Too_Long()
    {
        await Runner.RunScenarioAsync(
            when => UpdateShopReview_Is_Called(_shop.Id, _unknownReviewId, _tooLongRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(TextField, TextTooLongValidationMessage));
    }
}
