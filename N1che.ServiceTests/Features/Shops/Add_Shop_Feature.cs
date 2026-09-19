using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.ServiceTests.CommonSteps;

namespace N1che.ServiceTests.Features.Shops;

public partial class Add_Shop_Feature
{
    [Scenario]
    public async Task Add_Shop_Creates_The_Shop_With_The_Trading_Hours_Google_Holds()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(TradesToday),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Shop_Is_Returned(TradesToday.OpenTime, TradesToday.CloseTime),
            and => The_Stored_Trading_Hours_Are(TradesToday),
            and => Logs.There_Should_Be_A_Log(_endpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Stores_No_Trading_Hours_When_Google_Holds_None()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Shop_Is_Returned(openTime: null, closeTime: null),
            and => The_Stored_Trading_Hours_Are(),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Stores_One_Window_When_The_Place_Trades_In_Two_Periods_On_A_Day()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(MorningToday, AfternoonToday),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Shop_Is_Returned(MorningToday.OpenTime, AfternoonToday.CloseTime),
            and => The_Stored_Trading_Hours_Are(TradesAllDayToday));
    }

    [Scenario]
    public async Task Add_Shop_Closes_The_Window_At_Midnight_When_The_Place_Trades_Past_It()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(TradesPastMidnightToday),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Created),
            and => The_Created_Shop_Is_Returned(TradesUntilMidnightToday.OpenTime, TradesUntilMidnightToday.CloseTime),
            and => The_Stored_Trading_Hours_Are(TradesUntilMidnightToday));
    }

    [Scenario]
    public async Task Add_Shop_Returns_Conflict_When_The_Place_Has_Already_Been_Added()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(TradesToday),
            and => CreateShop_Is_Called(_request),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.Conflict),
            and => The_Response_Is_A_Problem(HttpStatusCode.Conflict, _duplicateShopMessage),
            and => One_Shop_Is_Stored());
    }

    [Scenario]
    public async Task Add_Shop_Returns_Bad_Request_When_Google_Has_No_Such_Place()
    {
        await Runner.RunScenarioAsync(
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, _rejectedPlaceMessage),
            and => No_Shop_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(_unknownPlaceLog, LogLevel.Warning, NoScopes, TestLogger),
            and => Logs.There_Should_Not_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Returns_Bad_Request_When_Google_Will_Not_Accept_The_Place_Id()
    {
        await Runner.RunScenarioAsync(
            given => Google_Will_Not_Accept_The_Place_Id(),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, _rejectedPlaceMessage),
            and => No_Shop_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(_unknownPlaceLog, LogLevel.Warning, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Returns_Bad_Gateway_When_Google_Cannot_Answer()
    {
        await Runner.RunScenarioAsync(
            given => Google_Rejects_Our_Api_Key(),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadGateway),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadGateway, GoogleUnavailableMessage),
            and => No_Shop_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(_googleFailureLog, LogLevel.Error, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Returns_Bad_Request_When_The_Place_Is_Permanently_Closed()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Is_Permanently_Closed(),
            when => CreateShop_Is_Called(_request),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, _rejectedPlaceMessage),
            and => No_Shop_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(_permanentlyClosedLog, LogLevel.Warning, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Returns_Bad_Request_When_A_Niche_Is_Not_Recognised()
    {
        await Runner.RunScenarioAsync(
            given => The_Place_Trades(TradesToday),
            when => CreateShop_Is_Called(_unrecognisedNicheRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Problem(HttpStatusCode.BadRequest, UnrecognisedNicheMessage),
            and => No_Shop_Is_Stored(),
            and => Logs.There_Should_Be_A_Log(UnrecognisedNicheLog, LogLevel.Warning, NoScopes, TestLogger));
    }

    [Scenario]
    public async Task Add_Shop_Returns_A_Validation_Error_When_No_Niche_Is_Given()
    {
        await Runner.RunScenarioAsync(
            when => CreateShop_Is_Called(_nicheLessRequest),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Is_A_Validation_Error_For(NichesField, NichesValidationMessage),
            and => No_Shop_Is_Stored());
    }
}
