using LightBDD.XUnit2;
using N1che.ServiceTests.Infrastructure;
using Testcontainers.PostgreSql;

[assembly: ConfigureLightBddScopeAttribute]

namespace N1che.ServiceTests.Infrastructure;

internal class ConfigureLightBddScopeAttribute : LightBddScopeAttribute
{
    private PostgreSqlContainer? _postgresContainer;

    protected override void OnSetUp()
    {
        _postgresContainer = new PostgreSqlBuilder("postgis/postgis:16-3.4")
            .Build();

        _postgresContainer.StartAsync().Wait();

        var connectionString = _postgresContainer.GetConnectionString();
        Environment.SetEnvironmentVariable("Postgres__ConnectionString", connectionString);
        DatabaseMigrator.ApplyAsync(connectionString).Wait();

        TestWebApplicationFactory.Initialize(_ => { });
    }

    protected override void OnTearDown()
    {
        TestWebApplicationFactory.Dispose(_ => { });
        _postgresContainer?.DisposeAsync().GetAwaiter().GetResult();
    }
}
