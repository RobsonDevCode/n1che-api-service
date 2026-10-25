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

internal static class UpdateShopReviewEndpoints
{
    internal static RouteGroupBuilder AddUpdateShopReviewEndpoints(this RouteGroupBuilder group)
    {
        group.MapPut("{shopId:guid}/reviews/{reviewId:guid}", Update)
            .WithValidation<UpdateReviewRequest>()
            .WithSummary("Update Shop Review")
            .WithDescription("Replace the text of a review the calling user wrote")
            .ProducesProblem(StatusCodes.Status404NotFound);

        return group;
    }

    private static async Task<Ok<ReviewResponse>> Update(
        [FromRoute] Guid shopId,
        [FromRoute] Guid reviewId,
        [FromBody] UpdateReviewRequest request,
        ClaimsPrincipal principal,
        [FromServices] IReviewUpdateService reviewUpdateService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var user = principal.ToUserModel();

        var logger = loggerFactory.CreateLogger("Update Shop Review");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = shopId,
            ["ReviewId"] = reviewId,
            ["UserId"] = user.Id
        });

        logger.LogInformation("Updating review {ReviewId} for shop {ShopId}", reviewId, shopId);

        var review = await reviewUpdateService.UpdateAsync(request.ToDomainModel(shopId, reviewId, user), cancellationToken);

        logger.LogInformation("Shop review updated");

        return TypedResults.Ok(review.ToResponse());
    }
}
