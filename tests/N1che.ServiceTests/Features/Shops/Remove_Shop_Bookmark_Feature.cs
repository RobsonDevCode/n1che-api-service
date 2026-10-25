using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Remove_Shop_Bookmark_Feature
{
    [Scenario]
    public async Task Remove_Shop_Bookmark_Returns_No_Content_When_The_Caller_Has_Bookmarked()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Is_Bookmarked_By(_callerId),
            when => RemoveShopBookmark_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Bookmarks_Are(),
            and => The_Shops_Vote_Count_Is_Unchanged(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Shop_Bookmark_Returns_No_Content_When_Another_User_Has_Also_Bookmarked()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Is_Bookmarked_By(_otherUserId),
            and => The_Shop_Is_Bookmarked_By(_callerId),
            when => RemoveShopBookmark_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NoContent),
            and => The_Stored_Bookmarks_Are(_otherUserId),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Shop_Bookmark_Returns_Not_Found_When_The_Caller_Has_Not_Bookmarked()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Is_Bookmarked_By(_otherUserId),
            when => RemoveShopBookmark_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _bookmarkNotFoundMessage),
            and => The_Stored_Bookmarks_Are(_otherUserId),
            and => Logs.There_Should_Be_A_Log(_bookmarkNotFoundMessage, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Remove_Shop_Bookmark_Returns_Not_Found_When_No_Shop_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => RemoveShopBookmark_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_A_Problem(HttpStatusCode.NotFound, _shopNotFoundMessage),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(_shopNotFoundMessage, LogLevel.Warning, NoScopes, TestLogger));
    }
}
