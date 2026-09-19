using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Api.Extensions.Shops;
using N1che.Api.Validation;
using N1che.Contracts.Requests.Shops;
using N1che.Contracts.Response.Shops;
using N1che.Domain.Interfaces.Services.Shops;

namespace N1che.Api.Endpoints.Shops;

internal static class CreateShopEndpoints
{
    internal static RouteGroupBuilder AddCreateShopEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("", Create)
            .WithValidation<CreateShopRequest>()
            .WithSummary("Add Shop")
            .WithDescription("Add a Google place as a shop, seeding its trading hours from Google Places");

        return group;
    }

    private static async Task<Created<ShopDetailResponse>> Create(
        [FromBody] CreateShopRequest request,
        ClaimsPrincipal principal,
        [FromServices] IShopCreationService shopCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Add Shop");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["GooglePlaceId"] = request.GooglePlaceId,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Adding shop for google place {GooglePlaceId}", request.GooglePlaceId);

        var shop = await shopCreationService.CreateAsync(request.ToDomainModel(user), cancellationToken);

        logger.LogInformation("Shop added");

        return TypedResults.Created($"/shops/{shop.Id}", shop.ToDetailResponse());
    }
}
