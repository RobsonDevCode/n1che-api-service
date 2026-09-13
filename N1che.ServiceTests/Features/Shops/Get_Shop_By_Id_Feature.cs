using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Shop_By_Id_Feature
{
    [Scenario]
    public async Task Get_Shop_By_Id_Returns_The_Shop_When_It_Exists_And_Trades_Today()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => The_Shop_Trades_Today(),
            when => GetShopById_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shop_Is_Returned(),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Shop_By_Id_Returns_The_Shop_Without_Hours_When_It_Does_Not_Trade_Today()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            when => GetShopById_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shop_Is_Returned());
    }

    [Scenario]
    public async Task Get_Shop_By_Id_Returns_Not_Found_When_No_Shop_Has_That_Id()
    {
        await Runner.RunScenarioAsync(
            when => GetShopById_Is_Called(Guid.NewGuid()),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.NotFound),
            and => The_Response_Is_Shop_Not_Found());
    }

    [Scenario]
    public async Task Get_Shop_By_Id_Returns_The_Cached_Shop_When_The_Row_Changes_Within_The_Cache_Lifetime()
    {
        await Runner.RunScenarioAsync(
            given => The_Shop_Exists(),
            and => GetShopById_Is_Called(_shop.Id),
            when => The_Shop_Is_Renamed_In_The_Database(),
            and => GetShopById_Is_Called(_shop.Id),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shop_Is_Returned());
    }
}
