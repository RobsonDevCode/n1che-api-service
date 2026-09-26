using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.ThirdParty.Google;

namespace N1che.Api.Extensions.ThirdPartyClients;

public static class GoogleRoutesClientExtensions
{
    private const string ConfigurationSection = "GoogleRoutes";
    private const string ApiKeyHeader = "X-Goog-Api-Key";

    public static IServiceCollection AddGoogleRoutesClient(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(ConfigurationSection);
        services.Configure<GoogleRoutesOptions>(section);

        var options = section.Get<GoogleRoutesOptions>()
            ?? throw new InvalidOperationException($"Missing '{ConfigurationSection}' configuration section");

        services.AddHttpClient<IGoogleRoutesClient, GoogleRoutesClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.TryAddWithoutValidation(ApiKeyHeader, options.ApiKey);
            })
            .AddStandardResilienceHandler();

        return services;
    }
}
