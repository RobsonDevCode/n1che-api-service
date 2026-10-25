using FluentValidation;
using N1che.Contracts.Requests.Routes;

namespace N1che.Api.Validation.Routes;

internal sealed class CreateRouteRequestValidator : AbstractValidator<CreateRouteRequest>
{
    private const int MaxNameLength = 100;
    private const int MaxTagLength = 50;
    private const int MaxNicheLength = 50;
    private const int MinPolylinePoints = 2;

    public CreateRouteRequestValidator()
    {
        // A required property binds as null, so nothing past the rule that catches it may count it.
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(MaxNameLength).WithMessage($"Name must be at most {MaxNameLength} characters.");

        RuleFor(request => request.Tag)
            .NotEmpty().WithMessage("Tag is required.")
            .MaximumLength(MaxTagLength).WithMessage($"Tag must be at most {MaxTagLength} characters.");

        RuleFor(request => request.Niche)
            .NotEmpty().WithMessage("Niche is required.")
            .MaximumLength(MaxNicheLength).WithMessage($"Niche must be at most {MaxNicheLength} characters.");

        RuleFor(request => request.Stops)
            .NotEmpty().WithMessage("Stops are required.")
            .Must(stops => stops.Count >= RouteStopRules.MinStops)
            .WithMessage($"A route needs at least {RouteStopRules.MinStops} stops.")
            .Must(stops => stops.Count <= RouteStopRules.MaxStops)
            .WithMessage(RouteStopRules.TooManyStops)
            .Must(stops => stops.Distinct().Count() == stops.Count)
            .WithMessage(RouteStopRules.RepeatedStop);

        RuleFor(request => request.Polyline)
            .NotEmpty().WithMessage("A polyline is required.")
            .Must(polyline => polyline.Count >= MinPolylinePoints)
            .WithMessage($"A polyline needs at least {MinPolylinePoints} points.");

        RuleForEach(request => request.Polyline).ChildRules(coordinate =>
        {
            coordinate.RuleFor(point => point.Latitude).MustBeALatitude();

            coordinate.RuleFor(point => point.Longitude).MustBeALongitude();
        });

        RuleFor(request => request.DistanceMeters)
            .GreaterThan(0).WithMessage("A route covers some distance.");

        RuleFor(request => request.TotalMinutes)
            .GreaterThanOrEqualTo(0).WithMessage("A route cannot take less than no time to walk.");
    }
}
