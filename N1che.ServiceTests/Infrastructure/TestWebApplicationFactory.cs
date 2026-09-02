using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            });
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
