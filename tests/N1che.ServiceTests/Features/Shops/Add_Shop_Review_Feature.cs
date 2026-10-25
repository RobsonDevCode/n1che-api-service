using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Add_Shop_Review_Feature
{
    [Scenario]
    public async Task Add_Shop_Review_Returns_The_Created_Review_When_The_Niche_Is_One_Of_The_Shops()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            when => CreateShopReview_Is_Called(_shop.Id, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Review_Is_Returned_And_Stored(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Review_Returns_Bad_Request_When_The_Niche_Is_Not_One_Of_The_Shops()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            when => CreateShopReview_Is_Called(_shop.Id, _otherNicheRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, _otherNicheMessage, _shop.Id),
            and => No_Review_Is_Stored(_shop.Id),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Review_Returns_Not_Found_When_No_Shop_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => CreateShopReview_Is_Called(_unknownShopId, _request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _notFoundMessage, _unknownShopId),
            and => No_Review_Is_Stored(_unknownShopId),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Review_Returns_A_Validation_Error_When_The_Text_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateShopReview_Is_Called(_unknownShopId, _textlessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(TextField, TextValidationMessage));
    }

    [Scenario]
    public async Task Add_Shop_Review_Returns_A_Validation_Error_When_The_Text_Is_Too_Long()
    {
        await Runner.RunScenarioAsync(
            when => CreateShopReview_Is_Called(_unknownShopId, _tooLongRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(TextField, TextTooLongValidationMessage));
    }

    [Scenario]
    public async Task Add_Shop_Review_Returns_A_Validation_Error_When_The_Niche_Is_Missing()
    {
        await Runner.RunScenarioAsync(
            when => CreateShopReview_Is_Called(_unknownShopId, _nicheLessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NicheField, NicheValidationMessage));
    }
}
