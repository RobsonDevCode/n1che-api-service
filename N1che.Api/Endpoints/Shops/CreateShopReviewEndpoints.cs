using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Authentication;
using N1che.Api.Extensions.Reviews;
using N1che.Api.Validation;
using N1che.Contracts.Requests.Reviews;
using N1che.Contracts.Response.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;

namespace N1che.Api.Endpoints.Shops;

internal static class CreateShopReviewEndpoints
{
    internal static RouteGroupBuilder AddCreateShopReviewEndpoints(this RouteGroupBuilder group)
    {
        group.MapPost("{id:guid}/reviews", Create)
            .WithValidation<CreateReviewRequest>()
            .WithSummary("Add Shop Review")
            .WithDescription("Review a shop in one of its niches on the calling user's behalf")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<Created<ReviewResponse>> Create(
        [FromRoute] Guid id,
        [FromBody] CreateReviewRequest request,
        ClaimsPrincipal principal,
        [FromServices] IReviewCreationService reviewCreationService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Add Shop Review");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["Niche"] = request.Niche,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Adding review for shop {ShopId} in niche {Niche}", id, request.Niche);

        var review = await reviewCreationService.CreateAsync(request.ToDomainModel(id, user), cancellationToken);

        logger.LogInformation("Shop review added");

        return TypedResults.Created($"/shops/{id}/reviews", review.ToResponse());
    }
}
