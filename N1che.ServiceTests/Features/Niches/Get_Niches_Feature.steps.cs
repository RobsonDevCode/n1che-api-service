using System.Net.Http.Json;
using AwesomeAssertions;
using LightBDD.XUnit2;
using N1che.Contracts.Response.Niches;
using N1che.ServiceTests.Infrastructure;
using N1che.ServiceTests.Infrastructure.Clients;
using N1che.ServiceTests.Infrastructure.Logger;

namespace N1che.ServiceTests.Features.Niches;

public partial class Get_Niches_Feature : FeatureFixture
{
    private readonly Dictionary<string, object> _scopeValues = new();
    private HttpResponseMessage _response = null!;

    private static FakeLoggerProvider TestLogger => TestWebApplicationFactory.Instance.FakeLogger;
    private static HttpClient Client => TestWebApplicationFactory.Instance.CreateAuthenticatedClient();

    private const string EndpointLog = "Getting niches";
    private const string SuccessLog = "Niches retrieved";

    private async Task GetNiches_Is_Called()
    {
        _response = await Client.GetNiches();
    }

    private static readonly IReadOnlyCollection<NicheResponse> ExpectedNiches =
    [
        new() { Id = "goth", Label = "Goth", SubLabel = "dark", Description = "Dark aesthetics, velvet, silver hardware" },
        new() { Id = "oldmoney", Label = "Old Money", SubLabel = "quiet luxury", Description = "Tailored cuts, cashmere, quiet luxury" },
        new() { Id = "skater", Label = "Skater", SubLabel = "streetwise", Description = "Baggy fits, graphic tees, low-tops" },
        new() { Id = "streetwear", Label = "Streetwear", SubLabel = "culture", Description = "Limited drops, hoodies, sneaker culture" },
        new() { Id = "cottagecore", Label = "Cottage", SubLabel = "soft", Description = "Floral prints, linen, handmade pieces" },
        new() { Id = "y2k", Label = "Y2K", SubLabel = "retro-future", Description = "Low rise, chrome, butterfly clips" },
        new() { Id = "techwear", Label = "Techwear", SubLabel = "utility", Description = "Utility, waterproof, tactical fits" },
        new() { Id = "vintage", Label = "Vintage", SubLabel = "archive", Description = "Deadstock, 80s/90s, thrift finds" }
    ];

    private async Task All_Seeded_Niches_Are_Returned()
    {
        var niches = await _response.Content.ReadFromJsonAsync<IReadOnlyCollection<NicheResponse>>();

        niches.Should().NotBeNull();
        niches.Should().BeEquivalentTo(ExpectedNiches);
    }
}
