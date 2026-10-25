using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Reviews;
using N1che.ServiceTests.CommonSteps;
using N1che.ServiceTests.Infrastructure;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Shop_Reviews_Feature
{
    [Scenario]
    public async Task Get_Shop_Reviews_Returns_Pages_Of_The_Niches_Reviews_Newest_First_When_The_Shop_Has_Reviews_In_That_Niche()
    {
        var filter = new ReviewsFilter { Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            given => Reviews_Exist(),
            when => GetShopReviews_Is_Called(filter, new PaginationFilter(Page, Size)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Expected_Page_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(EndpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger),
            and => GetShopReviews_Is_Called(filter, new PaginationFilter(Page: 2, Size: Size)),
            and => The_Expected_Page_Is_Returned());
    }

    [Scenario]
    public async Task Get_Shop_Reviews_Returns_An_Empty_Page_When_The_Shop_Has_No_Reviews_In_That_Niche()
    {
        var filter = new ReviewsFilter { Niche = NicheConstants.Skater };

        await Runner.RunScenarioAsync(
            given => Reviews_Exist(),
            when => GetShopReviews_Is_Called(filter, new PaginationFilter(Page, Size)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Expected_Page_Is_Returned());
    }

    [Scenario]
    public async Task Get_Shop_Reviews_Returns_Bad_Request_When_Niche_Is_Not_Supplied()
    {
        var filter = new ReviewsFilter { Niche = null! };

        await Runner.RunScenarioAsync(
            when => GetShopReviews_Is_Called(filter, new PaginationFilter()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }

    [Scenario]
    public async Task Get_Shop_Reviews_Returns_Bad_Request_When_Niche_Is_Empty()
    {
        var filter = new ReviewsFilter { Niche = "" };

        await Runner.RunScenarioAsync(
            when => GetShopReviews_Is_Called(filter, new PaginationFilter()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }
}
