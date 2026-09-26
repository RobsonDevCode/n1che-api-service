using FluentValidation;
using N1che.Contracts.Requests.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Validation.Routes;

internal sealed class ComputeRouteRequestValidator : AbstractValidator<ComputeRouteRequest>
{
    private const int MaxStops = 15;
    private const int MinWaypoints = 2;

    public ComputeRouteRequestValidator()
    {
        // A required property binds as null, so nothing past the rule that catches it may count it.
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Mode)
            .Must(mode => mode is RouteModes.You or RouteModes.Loop)
            .WithMessage($"Mode must be '{RouteModes.You}' or '{RouteModes.Loop}'.");

        RuleFor(request => request.Stops)
            .NotEmpty().WithMessage("At least one stop is required.")
            .Must(stops => stops.Count <= MaxStops)
            .WithMessage($"A route can hold at most {MaxStops} stops.")
            .Must(stops => stops.Distinct().Count() == stops.Count)
            .WithMessage("A stop can only appear once in a route.");

        RuleFor(request => request.Stops)
            .Must(stops => stops.Count >= MinWaypoints)
            .WithMessage("A loop needs at least two stops.")
            .When(request => request.Mode == RouteModes.Loop);

        RuleFor(request => request.Stops)
            .Must(stops => stops.Count >= MinWaypoints)
            .WithMessage("A route without an origin needs at least two stops.")
            .When(request => request.Mode == RouteModes.You && request.Origin is null);

        RuleFor(request => request.Origin)
            .Null()
            .WithMessage("A loop starts at its first stop, so it cannot walk from an origin.")
            .When(request => request.Mode == RouteModes.Loop);

        When(request => request.Origin is not null, () =>
        {
            RuleFor(request => request.Origin!.Latitude).MustBeALatitude();

            RuleFor(request => request.Origin!.Longitude).MustBeALongitude();
        });
    }
}
