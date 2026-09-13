using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.Contracts.Filters.Routes;
using N1che.ServiceTests.CommonSteps;
using N1che.ServiceTests.Infrastructure;

namespace N1che.ServiceTests.Features.Routes;

public partial class Get_Routes_Feature
{
    [Scenario]
    public async Task Get_Routes_Returns_Routes_When_Routes_Exist_In_Radius_Filtered_By_Niche_Most_Upvoted_First()
    {
        var filter = new RoutesFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            given => Routes_Exist(),
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Routes_Matching_The_Filter_Are_Returned(),
            and => Logs.There_Should_Be_A_Log(EndpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Routes_Excluding_Those_Outside_The_Radius()
    {
        var filter = new RoutesFilter
        {
            Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth, Radius = SmallRadiusMeters
        };

        await Runner.RunScenarioAsync(
            given => Routes_Exist(),
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Routes_Matching_The_Filter_Are_Returned());
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Niche_Is_Not_Supplied()
    {
        var filter = new RoutesFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = null! };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Only_The_Highest_Rated_Routes_When_A_Limit_Is_Supplied()
    {
        var filter = new RoutesFilter
        {
            Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth, Limit = 2
        };

        await Runner.RunScenarioAsync(
            given => Routes_Exist(),
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Routes_Matching_The_Filter_Are_Returned());
    }

    [Scenario]
    public async Task Get_Routes_Returns_An_Empty_Array_When_No_Routes_Exist_In_The_Radius()
    {
        var filter = new RoutesFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Routes_Matching_The_Filter_Are_Returned());
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Latitude_Is_Out_Of_Range()
    {
        var filter = new RoutesFilter { Lat = 200, Lng = _originLongitude, Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Lat"));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Longitude_Is_Out_Of_Range()
    {
        var filter = new RoutesFilter { Lat = _originLatitude, Lng = 200, Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Lng"));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Radius_Is_Not_Positive()
    {
        var filter = new RoutesFilter
        {
            Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth, Radius = 0
        };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Radius"));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Limit_Exceeds_Maximum()
    {
        var filter = new RoutesFilter
        {
            Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth, Limit = 500
        };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Limit"));
    }

    [Scenario]
    public async Task Get_Routes_Returns_Bad_Request_When_Niche_Is_Empty()
    {
        var filter = new RoutesFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = "" };

        await Runner.RunScenarioAsync(
            when => GetRoutes_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }
}
