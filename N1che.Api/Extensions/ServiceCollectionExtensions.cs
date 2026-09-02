using Dapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using N1che.Api.Validation.Shops;
using N1che.Contracts.Filters.Shops;
using N1che.Domain.Interfaces;
using N1che.Domain.Interfaces.Persistence.Readers.Niches;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Services.Niches;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Services.Niches;
using N1che.Domain.Services.Shops;
using N1che.Persistence.Postgres.Postgres.Configuration;
using N1che.Persistence.Postgres.Postgres.Connections;
using N1che.Persistence.Postgres.Postgres.Readers.Niches;
using N1che.Persistence.Postgres.Postgres.Readers.Shops;
using N1che.Persistence.Postgres.Postgres.Transactions;
using Npgsql;

namespace N1che.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        services.Configure<PostgresOptions>(configuration.GetSection("Postgres"));

        services.AddSingleton<NpgsqlDataSource>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;
            return NpgsqlDataSource.Create(options.ConnectionString);
        });

        services.AddScoped<PostgresTransactionContext>();
        services.AddScoped<ITransactionScope, PostgresTransactionScope>();
        services.AddScoped<PostgresConnectionFactory>();
        services.AddScoped<INichesReader, NicheReader>();
        services.AddScoped<IShopsReader, ShopReader>();

        return services;
    }

    public static IServiceCollection AddDomainDependencies(this IServiceCollection services)
    {
        services.AddScoped<INicheRetrievalService, NicheRetrievalService>();
        services.AddScoped<IShopRetrievalService, ShopRetrievalService>();

        return services;
    }

    public static IServiceCollection AddValidationDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<NearbyShopsFilter>, NearbyShopsFilterValidator>();

        return services;
    }
}
