using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Shops;
using N1che.ServiceTests.CommonSteps;
using N1che.ServiceTests.Infrastructure;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Paged_Shops_Feature
{
    [Scenario]
    public async Task Get_Paged_Shops_Returns_Pages_Ordered_By_Popularity_For_A_Niche()
    {
        var filter = new ShopsFilter { Niche = [NicheConstants.Cottagecore] };

        await Runner.RunScenarioAsync(
            given => Shops_Exist(NicheConstants.Cottagecore),
            when => GetShopsPage_Is_Called(filter, new PaginationFilter(Page, Size)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Expected_Page_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(EndpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger),
            and => GetShopsPage_Is_Called(filter, new PaginationFilter(Page: 2, Size: Size)),
            and => The_Expected_Page_Is_Returned());
    }

    [Scenario]
    public async Task Get_Paged_Shops_Returns_Shops_Ordered_By_Popularity_When_No_Niche_Is_Given()
    {
        var pagination = new PaginationFilter(Page: 1, Size: SeededShopCount);

        await Runner.RunScenarioAsync(
            given => Shops_Exist(NicheConstants.WesternWear),
            when => GetShopsPage_Is_Called(new ShopsFilter(), pagination),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Returned_Page_Is_Ordered_By_Popularity());
    }

    [Scenario]
    public async Task Get_Paged_Shops_Returns_Bad_Request_When_Niche_Is_Empty()
    {
        var filter = new ShopsFilter { Niche = [""] };

        await Runner.RunScenarioAsync(
            when => GetShopsPage_Is_Called(filter, new PaginationFilter()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }

    [Scenario]
    public async Task Get_Paged_Shops_Returns_Bad_Request_When_Page_Is_Less_Than_One()
    {
        await Runner.RunScenarioAsync(
            when => GetShopsPage_Is_Called(new ShopsFilter(), new PaginationFilter(Page: 0, Size: 10)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Page"));
    }

    [Scenario]
    public async Task Get_Paged_Shops_Returns_Bad_Request_When_Size_Exceeds_Maximum()
    {
        await Runner.RunScenarioAsync(
            when => GetShopsPage_Is_Called(new ShopsFilter(), new PaginationFilter(Page: 1, Size: 500)),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Size"));
    }
}
