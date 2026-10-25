using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using N1che.Api.Extensions.Niches;
using N1che.Contracts.Response.Niches;
using N1che.Domain.Interfaces.Services.Niches;

namespace N1che.Api.Endpoints.Niches;

internal static class GetNichesEndpoints
{
    internal static RouteGroupBuilder AddGetNichesEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("", GetAll)
            .WithSummary("Get Niches")
            .WithDescription("Get all supported niches");

        return group;
    }

    private static async Task<Ok<IReadOnlyCollection<NicheResponse>>> GetAll(
        [FromServices] INicheRetrievalService nicheRetrievalService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("Get Niches");

        logger.LogInformation("Getting niches");

        var niches = await nicheRetrievalService.GetAllAsync(cancellationToken);

        logger.LogInformation("Niches retrieved");

        return TypedResults.Ok(niches.ToResponse());
    }
}
