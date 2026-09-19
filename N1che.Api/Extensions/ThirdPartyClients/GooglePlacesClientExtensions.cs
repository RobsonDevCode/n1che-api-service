using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.ThirdParty.Google;

namespace N1che.Api.Extensions.ThirdPartyClients;

public static class GooglePlacesClientExtensions
{
    private const string ConfigurationSection = "GooglePlaces";
    private const string ApiKeyHeader = "X-Goog-Api-Key";

    public static IServiceCollection AddGooglePlacesClient(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(ConfigurationSection);
        services.Configure<GooglePlacesOptions>(section);

        var options = section.Get<GooglePlacesOptions>()
            ?? throw new InvalidOperationException($"Missing '{ConfigurationSection}' configuration section");

        services.AddHttpClient<IGooglePlacesClient, GooglePlacesClient>(client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.TryAddWithoutValidation(ApiKeyHeader, options.ApiKey);
            })
            .AddStandardResilienceHandler();

        return services;
    }
}
