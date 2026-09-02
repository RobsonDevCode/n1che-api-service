using N1che.Contracts.Filters.Shops;

namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class ShopsClient
{
    public static async Task<HttpResponseMessage> GetNearbyShops(this HttpClient client, NearbyShopsFilter filter)
    {
        var query = new QueryBuilder()
            .Add("lat", filter.Lat)
            .Add("lng", filter.Lng)
            .Add("radius", filter.Radius)
            .Add("niche", filter.Niche)
            .Add("limit", filter.Limit)
            .Build();

        return await client.GetAsync($"shops/nearby{query}");
    }
}
