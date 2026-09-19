using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Places;
using N1che.Api.Validation;
using N1che.Contracts.Filters.Places;
using N1che.Contracts.Response.Places;
using N1che.Domain.Interfaces.Services.Places;

namespace N1che.Api.Endpoints.Places;

internal static class SearchPlacesEndpoints
{
    internal static RouteGroupBuilder AddSearchPlacesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("search", Search)
            .WithValidation<PlacesSearchFilter>()
            .WithSummary("Search Places")
            .WithDescription("Search Google Places within an area for a place to add as a shop");

        return group;
    }

    private static async Task<Ok<IReadOnlyCollection<PlaceResponse>>> Search(
        [AsParameters] PlacesSearchFilter filter,
        [FromServices] IPlaceRetrievalService placeRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Search Places");
        using var _ = logger.BeginScope(new Dictionary<string, object> { ["Query"] = filter.Query });

        logger.LogInformation("Searching places for {Query}", filter.Query);

        var places = await placeRetrievalService.SearchAsync(filter.ToDomainFilter(), cancellationToken);

        logger.LogInformation("Places retrieved");

        return TypedResults.Ok(places.ToResponse());
    }
}
