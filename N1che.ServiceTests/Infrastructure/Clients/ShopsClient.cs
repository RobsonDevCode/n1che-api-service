using N1che.Contracts.Filters.Pagination;
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

    public static async Task<HttpResponseMessage> GetShopsPage(this HttpClient client, ShopsFilter filter, PaginationFilter pagination)
    {
        var builder = new QueryBuilder();
        foreach (var niche in filter.Niche ?? [])
        {
            builder.Add("niche", niche);
        }

        var query = builder
            .Add("page", pagination.Page)
            .Add("size", pagination.Size)
            .Build();

        return await client.GetAsync($"shops{query}");
    }
}
