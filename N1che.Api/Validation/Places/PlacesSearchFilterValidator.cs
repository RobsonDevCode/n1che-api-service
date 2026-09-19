using FluentValidation;
using N1che.Contracts.Filters.Places;

namespace N1che.Api.Validation.Places;

internal sealed class PlacesSearchFilterValidator : AbstractValidator<PlacesSearchFilter>
{
    private const int MaxQueryLength = 200;
    private const double MaxLatitudeSpan = 2;
    private const double MaxLongitudeSpan = 2;
    private const double DegreesInACircle = 360;

    public PlacesSearchFilterValidator()
    {
        RuleFor(filter => filter.Query)
            .NotEmpty().WithMessage("Query is required.")
            .MaximumLength(MaxQueryLength).WithMessage($"Query must be at most {MaxQueryLength} characters.");

        RuleFor(filter => filter.SwLat).MustBeALatitude();

        RuleFor(filter => filter.NeLat).MustBeALatitude();

        RuleFor(filter => filter.SwLng).MustBeALongitude();

        RuleFor(filter => filter.NeLng).MustBeALongitude();

        RuleFor(filter => filter.NeLat)
            .GreaterThan(filter => filter.SwLat)
            .WithMessage("The north-east corner must be north of the south-west corner.");

        RuleFor(filter => filter.NeLat)
            .Must((filter, neLat) => neLat - filter.SwLat <= MaxLatitudeSpan)
            .WithMessage($"The search area must span at most {MaxLatitudeSpan} degrees of latitude.")
            .When(filter => filter.NeLat > filter.SwLat);

        RuleFor(filter => filter.NeLng)
            .Must((filter, _) => LongitudeSpan(filter) > 0)
            .WithMessage("The north-east corner must be east of the south-west corner.");

        RuleFor(filter => filter.NeLng)
            .Must((filter, _) => LongitudeSpan(filter) <= MaxLongitudeSpan)
            .WithMessage($"The search area must span at most {MaxLongitudeSpan} degrees of longitude.")
            .When(filter => LongitudeSpan(filter) > 0);
    }

    // An area crossing the antimeridian has its corners inverted, so the span wraps rather than running
    // backwards. The one span that wraps to nothing, 180 east to 180 west, is the area Google rejects.
    private static double LongitudeSpan(PlacesSearchFilter filter)
    {
        var span = filter.NeLng - filter.SwLng;
        return span < 0 ? span + DegreesInACircle : span;
    }
}
