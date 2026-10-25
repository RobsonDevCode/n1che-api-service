using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using N1che.Api.Extensions;

namespace N1che.Api.Endpoints.Health;

internal static class GetHealthEndpoints
{
    private const string NotReadyMessage = "Service is not ready";

    internal static RouteGroupBuilder AddGetHealthEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("live", GetLiveness)
            .WithSummary("Get Liveness")
            .WithDescription("Report that the process is up, without checking its dependencies");

        group.MapGet("ready", GetReadiness)
            .WithSummary("Get Readiness")
            .WithDescription("Report whether the service's dependencies are reachable")
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return group;
    }

    private static Ok GetLiveness() => TypedResults.Ok();

    private static async Task<Results<Ok, ProblemHttpResult>> GetReadiness(
        [FromServices] HealthCheckService healthCheckService,
        [FromServices] ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var report = await healthCheckService.CheckHealthAsync(
            check => check.Tags.Contains(HealthCheckExtensions.ReadyTag),
            cancellationToken);

        if (report.Status != HealthStatus.Unhealthy)
        {
            return TypedResults.Ok();
        }

        var failedChecks = report.Entries
            .Where(entry => entry.Value.Status == HealthStatus.Unhealthy)
            .Select(entry => entry.Key);

        loggerFactory.CreateLogger("Get Readiness")
            .LogWarning("Readiness checks failed: {FailedChecks}", string.Join(", ", failedChecks));

        return TypedResults.Problem(statusCode: StatusCodes.Status503ServiceUnavailable, detail: NotReadyMessage);
    }
}
