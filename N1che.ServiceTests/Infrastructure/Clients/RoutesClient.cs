using N1che.Contracts.Filters.Routes;

namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class RoutesClient
{
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
