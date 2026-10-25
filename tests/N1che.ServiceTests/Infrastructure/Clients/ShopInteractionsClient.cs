namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class ShopInteractionsClient
{
    public static async Task<HttpResponseMessage> GetShopInteractions(this HttpClient client, Guid id)
    {
        return await client.GetAsync($"shops/{id}/interactions");
    }

    public static async Task<HttpResponseMessage> AddShopVote(this HttpClient client, Guid id)
    {
        return await client.PostAsync($"shops/{id}/vote", content: null);
    }

    public static async Task<HttpResponseMessage> RemoveShopVote(this HttpClient client, Guid id)
    {
        return await client.DeleteAsync($"shops/{id}/vote");
    }

    public static async Task<HttpResponseMessage> AddShopBookmark(this HttpClient client, Guid id)
    {
        return await client.PostAsync($"shops/{id}/save", content: null);
    }

    public static async Task<HttpResponseMessage> RemoveShopBookmark(this HttpClient client, Guid id)
    {
        return await client.DeleteAsync($"shops/{id}/save");
    }
}
