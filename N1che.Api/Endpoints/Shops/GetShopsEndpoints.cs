using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Shops;
using N1che.Api.Validation;
using N1che.Contracts.Filters.Pagination;
using N1che.Contracts.Filters.Shops;
using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Shops;
using N1che.Domain.Interfaces.Services.Shops;
using N1che.Domain.Models.Pagination;

namespace N1che.Api.Endpoints.Shops;

internal static class GetShopsEndpoints
{
    internal static RouteGroupBuilder AddGetShopsEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetPage)
            .WithValidation<ShopsFilter>()
            .WithValidation<PaginationFilter>()
            .WithSummary("Get Shops")
            .WithDescription("Get a page of shops ordered most-popular first, optionally filtered by niche");

        group.MapGet("nearby", GetNearby)
            .WithValidation<NearbyShopsFilter>()
            .WithSummary("Get Nearby Shops")
            .WithDescription("Get shops within a radius of a location, optionally filtered by niche, nearest first");

        group.MapGet("{id:guid}", GetById)
            .WithSummary("Get Shop By Id")
            .WithDescription("Get a single shop's static detail; votes and reviews are served by their own endpoints");

        return group;
    }

    private static async Task<Ok<PagedResponse<ShopResponse>>> GetPage(
        [AsParameters] ShopsFilter filter,
        [AsParameters] PaginationFilter pagination,
        [FromServices] IShopRetrievalService shopRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Shops");
        
        var nicheLabel = string.Join(" | ", filter.Niche ?? ["all"]);
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["Niche"] = nicheLabel,
            ["Page"] = pagination.Page,
            ["Size"] = pagination.Size
        });

        logger.LogInformation("Getting page {Page} of shops (size {Size}) for niche {Niche}",
            pagination.Page, pagination.Size, nicheLabel);

        var page = await shopRetrievalService.GetPageAsync(
            filter.ToDomainFilter(), new PaginationDetailsModel(pagination.Page, pagination.Size), cancellationToken);

        logger.LogInformation("Shops page retrieved");

        return TypedResults.Ok(page.ToPagedResponse());
    }

    private static async Task<Ok<ShopDetailResponse>> GetById(
        [FromRoute] Guid id,
        [FromServices] IShopRetrievalService shopRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Shop By Id");
        using var _ = logger.BeginScope(new Dictionary<string, object> { ["ShopId"] = id });

        logger.LogInformation("Getting shop {ShopId}", id);

        var shop = await shopRetrievalService.GetByIdAsync(id, cancellationToken);

        logger.LogInformation("Shop retrieved");

        return TypedResults.Ok(shop.ToDetailResponse());
    }

    private static async Task<Ok<IReadOnlyCollection<ShopResponse>>> GetNearby(
        [AsParameters] NearbyShopsFilter filter,
        [FromServices] IShopRetrievalService shopRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Nearby Shops");
        using var _ = logger.BeginScope(new Dictionary<string, object>
        {
            ["Latitude"] = filter.Lat,
            ["Longitude"] = filter.Lng,
            ["Niche"] = filter.Niche ?? "all"
        });

        logger.LogInformation("Getting nearby shops at ({Latitude}, {Longitude}) for niche {Niche}",
            filter.Lat, filter.Lng, filter.Niche ?? "all");

        var shops = await shopRetrievalService.GetNearbyAsync(filter.ToDomainFilter(), cancellationToken);
        
        logger.LogInformation("Nearby shops retrieved");

        return TypedResults.Ok(shops.ToResponse());
    }
}
