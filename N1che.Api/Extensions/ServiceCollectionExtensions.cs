using Dapper;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using N1che.Api.Authentication;
using N1che.Api.Validation.Pagination;
using N1che.Api.Validation.Shops;
using N1che.Contracts.Filters.Pagination;
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

    public static IServiceCollection AddCognitoAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Cognito");
        services.Configure<CognitoOptions>(section);

        var cognito = section.Get<CognitoOptions>()
            ?? throw new InvalidOperationException("Missing 'Cognito' configuration section");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = cognito.Authority;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = cognito.Authority,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "username"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => ValidateCognitoAccessToken(context, cognito.ClientId)
                };
            });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddValidationDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<NearbyShopsFilter>, NearbyShopsFilterValidator>();
        services.AddSingleton<IValidator<ShopsFilter>, ShopsFilterValidator>();
        services.AddSingleton<IValidator<PaginationFilter>, PaginationFilterValidator>();

        return services;
    }

    // Cognito access tokens carry no `aud`; authorization is asserted by `token_use` and `client_id`.
    private static Task ValidateCognitoAccessToken(TokenValidatedContext context, string expectedClientId)
    {
        var tokenUse = context.Principal?.FindFirst("token_use")?.Value;
        if (!string.Equals(tokenUse, "access", StringComparison.Ordinal))
        {
            context.Fail("Only Cognito access tokens are accepted");
            return Task.CompletedTask;
        }

        var clientId = context.Principal?.FindFirst("client_id")?.Value;
        if (!string.Equals(clientId, expectedClientId, StringComparison.Ordinal))
        {
            context.Fail("Token was issued for a different app client");
        }

        return Task.CompletedTask;
    }
}
