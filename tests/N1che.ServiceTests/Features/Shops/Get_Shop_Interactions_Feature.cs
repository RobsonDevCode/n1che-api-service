using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Shop_Interactions_Feature
{
    [Scenario]
    public async Task Get_Shop_Interactions_Returns_The_Callers_Own_Vote_And_Bookmark_State()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Is_Voted_For_By(_callerId),
            and => The_Shop_Is_Bookmarked_By(_callerId),
            when => GetShopInteractions_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: true, saved: true),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Shop_Interactions_Ignores_Another_Users_Vote_And_Bookmark()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Is_Voted_For_By(_otherUserId),
            and => The_Shop_Is_Bookmarked_By(_otherUserId),
            when => GetShopInteractions_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: false, saved: false),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Shop_Interactions_Returns_The_Live_Vote_Count_When_It_Changes_Between_Calls()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => GetShopInteractions_Is_Called(_shop.Id),
            when => The_Shops_Vote_Count_Changes(),
            and => GetShopInteractions_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Interactions_Are_Returned(voted: false, saved: false),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Shop_Interactions_Returns_Not_Found_When_No_Shop_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => GetShopInteractions_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_Shop_Not_Found(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_notFoundLog, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }
}
