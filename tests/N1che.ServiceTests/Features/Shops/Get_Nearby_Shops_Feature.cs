using System.Net;
using LightBDD.Framework.Scenarios;
using LightBDD.XUnit2;
using Microsoft.Extensions.Logging;
using N1che.Contracts.Filters.Shops;
using N1che.ServiceTests.CommonSteps;
using N1che.ServiceTests.Infrastructure;

namespace N1che.ServiceTests.Features.Shops;

public partial class Get_Nearby_Shops_Feature
{
    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Shops_When_Shops_Exists_In_Radius_Filtered_By_Niche_Nearest_First()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Goth };

        await Runner.RunScenarioAsync(
            given => Shops_Exist(),
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shops_Matching_The_Filter_Are_Returned(),
            and => Logs.There_Should_Be_A_Log(EndpointLog, LogLevel.Information, _scopeValues, TestLogger),
            and => Logs.There_Should_Be_A_Log(SuccessLog, LogLevel.Information, _scopeValues, TestLogger));
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Shops_When_Shops_Exists_Excluding_Those_Outside_The_Radius()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Vintage, Radius = SmallRadiusMeters };

        await Runner.RunScenarioAsync(
            given => Shops_Exist(),
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shops_Matching_The_Filter_Are_Returned());
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Shops_When_Shops_Exists_Belonging_To_Multiple_Niches()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = NicheConstants.Streetwear };

        await Runner.RunScenarioAsync(
            given => Shops_Exist(),
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.OK),
            and => The_Shops_Matching_The_Filter_Are_Returned());
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Bad_Request_When_Latitude_Is_Out_Of_Range()
    {
        var filter = new NearbyShopsFilter { Lat = 200, Lng = _originLongitude };

        await Runner.RunScenarioAsync(
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Lat"));
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Bad_Request_When_Longitude_Is_Out_Of_Range()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = 200 };

        await Runner.RunScenarioAsync(
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Lng"));
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Bad_Request_When_Radius_Is_Not_Positive()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Radius = 0 };

        await Runner.RunScenarioAsync(
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Radius"));
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Bad_Request_When_Limit_Exceeds_Maximum()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Limit = 500 };

        await Runner.RunScenarioAsync(
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Limit"));
    }

    [Scenario]
    public async Task Get_Nearby_Shops_Returns_Bad_Request_When_Niche_Is_Empty()
    {
        var filter = new NearbyShopsFilter { Lat = _originLatitude, Lng = _originLongitude, Niche = "" };

        await Runner.RunScenarioAsync(
            when => GetNearbyShops_Is_Called(filter),
            then => HttpResponse.Status_Code_Is(_response, HttpStatusCode.BadRequest),
            and => The_Response_Contains_A_Validation_Error_For("Niche"));
    }
}
