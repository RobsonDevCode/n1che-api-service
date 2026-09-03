using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using N1che.ServiceTests.Infrastructure.Logger;

namespace N1che.ServiceTests.Infrastructure;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static TestWebApplicationFactory? s_instance;
    private static Exception? s_exception;
    public FakeLoggerProvider FakeLogger { get; } = new();

    private TestWebApplicationFactory() { }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("local")
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddProvider(FakeLogger);
            })
            .ConfigureServices(services =>
            {
                services.AddSingleton(FakeLogger);
                services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = null;
                    options.MetadataAddress = null!;
                    options.RequireHttpsMetadata = false;
                    options.Configuration = new OpenIdConnectConfiguration();
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = TestAuth.Issuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = TestAuth.SigningKey,
                        NameClaimType = "username"
                    };
                });
            });
    }

    public HttpClient CreateAuthenticatedClient(string? token = null)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token ?? TestAuth.GenerateToken());
        return client;
    }

    public static TestWebApplicationFactory Instance
    {
        get
        {
            if (s_instance is not null)
            {
                return s_instance;
            }

            throw s_exception ?? throw new Exception("Server failed to initialise");
        }
    }

    public static void Initialize(Action<IServiceProvider> action)
    {
        try
        {
            s_instance = new();
            s_instance.CreateDefaultClient();

            action(s_instance.Services);
        }
        catch (Exception ex)
        {
            s_exception = new Exception("Server failed to initialise", ex);
        }
    }

    public static void Dispose(Action<IServiceProvider> action)
    {
        if (s_instance is null)
        {
            throw new Exception("Cannot dispose null server instance");
        }

        action(s_instance.Services);
        s_instance.Dispose();
    }
}
