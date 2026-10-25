using Microsoft.Extensions.Options;
using N1che.Persistence.Postgres.Postgres.Configuration;

namespace N1che.Api.Extensions;

public static class HealthCheckExtensions
{
    internal const string ReadyTag = "ready";

    private static readonly string[] PostgresTags = ["db", "postgres", "npgsql", ReadyTag];

    public static IServiceCollection AddN1cheHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                sp => sp.GetRequiredService<IOptions<PostgresOptions>>().Value.ConnectionString,
                name: "postgres",
                tags: PostgresTags);

        return services;
    }
}
