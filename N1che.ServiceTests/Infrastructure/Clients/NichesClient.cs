namespace N1che.ServiceTests.Infrastructure.Clients;

internal static class NichesClient
{
    public static async Task<HttpResponseMessage> GetNiches(this HttpClient client)
    {
        return await client.GetAsync("niches");
    }
}
