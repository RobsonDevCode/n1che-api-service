using Microsoft.Extensions.Options;
using N1che.Domain.Interfaces;
using N1che.Persistence.Postgres.Postgres.Configuration;
using N1che.Persistence.Postgres.Postgres.Transactions;
using Npgsql;

namespace N1che.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetSection("Postgres"));

        services.AddSingleton<NpgsqlDataSource>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;
            return NpgsqlDataSource.Create(options.ConnectionString);
        });

        services.AddScoped<PostgresTransactionContext>();
        services.AddScoped<ITransactionScope, PostgresTransactionScope>();

        return services;
    }

    public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
    {
        return services;
    }
}
