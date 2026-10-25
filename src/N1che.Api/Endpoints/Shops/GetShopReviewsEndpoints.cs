using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Reviews;
using N1che.Api.Validation;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Reviews;
using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;
using N1che.Domain.Models.Pagination;

namespace N1che.Api.Endpoints.Shops;

internal static class GetShopReviewsEndpoints
{
    internal static RouteGroupBuilder AddGetShopReviewsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{id:guid}/reviews", GetPageByShopId)
            .WithValidation<ReviewsFilter>()
            .WithValidation<PaginationFilter>()
            .WithSummary("Get Shop Reviews")
            .WithDescription("Get a page of a shop's reviews in one niche, newest first");

        return group;
    }

    private static async Task<Ok<PagedResponse<ReviewResponse>>> GetPageByShopId(
        [FromRoute] Guid id,
        [AsParameters] ReviewsFilter filter,
        [AsParameters] PaginationFilter pagination,
        [FromServices] IReviewRetrievalService reviewRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Shop Reviews");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["ShopId"] = id,
            ["Niche"] = filter.Niche,
            ["Page"] = pagination.Page,
            ["Size"] = pagination.Size
        });

        logger.LogInformation("Getting page {Page} of reviews (size {Size}) for shop {ShopId} in niche {Niche}",
            pagination.Page, pagination.Size, id, filter.Niche);

        var page = await reviewRetrievalService.GetPageByShopIdAsync(
            id, filter.Niche, new PaginationDetailsModel(pagination.Page, pagination.Size), cancellationToken);

        logger.LogInformation("Shop reviews page retrieved");

        return TypedResults.Ok(page.ToPagedResponse());
    }
}
