using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Places;
using N1che.Api.Validation;
using N1che.Contracts.Filters.Places;
using N1che.Contracts.Response.Places;
using N1che.Domain.Interfaces.Services.Places;

namespace N1che.Api.Endpoints.Places;

internal static class GetPlacePhotoMediaEndpoints
{
    internal static RouteGroupBuilder AddGetPlacePhotoMediaEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("{placeId}/photos/{photoId}/media", GetPhotoMedia)
            .WithValidation<PlacePhotoMediaFilter>()
            .WithSummary("Get Place Photo Media")
            .WithDescription("Get a short-lived public address for a place photo's image");

        return group;
    }

    private static async Task<Ok<PlacePhotoMediaResponse>> GetPhotoMedia(
        [AsParameters] PlacePhotoMediaFilter filter,
        [FromServices] IPlaceRetrievalService placeRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var domainFilter = filter.ToDomainFilter();

        var logger = loggerFactory.CreateLogger("Get Place Photo Media");
        using var _ = logger.BeginScope(new Dictionary<string, object> { ["PhotoReference"] = domainFilter.Reference });

        logger.LogInformation("Getting photo {PhotoReference}", domainFilter.Reference);

        var media = await placeRetrievalService.GetPhotoMediaAsync(domainFilter, cancellationToken);

        logger.LogInformation("Photo retrieved");

        return TypedResults.Ok(media.ToResponse());
    }
}
