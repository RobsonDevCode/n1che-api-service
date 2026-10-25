using N1che.Contracts.Filters.Places;

namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class PlacesClient
{
    public static async Task<HttpResponseMessage> SearchPlaces(this HttpClient client, PlacesSearchFilter filter)
    {
        var query = new QueryBuilder()
            .Add("query", filter.Query)
            .Add("swLat", filter.SwLat)
            .Add("swLng", filter.SwLng)
            .Add("neLat", filter.NeLat)
            .Add("neLng", filter.NeLng)
            .Build();

        return await client.GetAsync($"places/search{query}");
    }

    public static async Task<HttpResponseMessage> GetPlacePhotoMedia(this HttpClient client, PlacePhotoMediaFilter filter)
    {
        var query = new QueryBuilder()
            .Add("maxWidth", filter.MaxWidth)
            .Build();

        return await client.GetAsync($"places/{filter.PlaceId}/photos/{filter.PhotoId}/media{query}");
    }
}
