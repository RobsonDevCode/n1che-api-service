using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace N1che.Api.OpenApi;

/// <summary>The Cognito bearer token Scalar sends to the endpoints that require authorisation.</summary>
internal static class BearerSecurity
{
    private const string SchemeName = "Bearer";

    internal static OpenApiOptions AddBearerSecurity(this OpenApiOptions options) =>
        options
            .AddDocumentTransformer((document, _, _) =>
            {
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Cognito JWT issued to the n1che-mobile app client"
                };

                return Task.CompletedTask;
            })
            .AddOperationTransformer((operation, context, _) =>
            {
                if (!context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any())
                {
                    return Task.CompletedTask;
                }

                operation.Security ??= [];
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(SchemeName, context.Document)] = []
                });

                return Task.CompletedTask;
            });
}
