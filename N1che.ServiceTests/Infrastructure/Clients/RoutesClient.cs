using System.Net.Http.Json;
using N1che.Contracts.Filters.Routes;
using N1che.Contracts.Requests.Routes;

namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class RoutesClient
{
    public static async Task<HttpResponseMessage> ComputeRoute(this HttpClient client, ComputeRouteRequest request) =>
        await client.PostAsJsonAsync("routes/compute", request);

    public static async Task<HttpResponseMessage> GetRoutes(this HttpClient client, RoutesFilter filter)
    {
        var query = new QueryBuilder()
            .Add("lat", filter.Lat)
            .Add("lng", filter.Lng)
            .Add("radius", filter.Radius)
            .Add("niche", filter.Niche)
            .Add("limit", filter.Limit)
            .Build();

        return await client.GetAsync($"routes{query}");
    }
}
