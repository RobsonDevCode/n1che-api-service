using FluentValidation;
using N1che.Contracts.Filters.Shops;

namespace N1che.Api.Validation.Shops;

internal sealed class NearbyShopsFilterValidator : AbstractValidator<NearbyShopsFilter>
{
    private const double MinLatitude = -90;
    private const double MaxLatitude = 90;
    private const double MinLongitude = -180;
    private const double MaxLongitude = 180;
    private const double MinRadiusMeters = 1;
    private const double MaxRadiusMeters = 50_000;
    private const int MinLimit = 1;
    private const int MaxLimit = 100;
    private const int MaxNicheLength = 50;

    public NearbyShopsFilterValidator()
    {
        RuleFor(filter => filter.Lat)
            .InclusiveBetween(MinLatitude, MaxLatitude)
            .WithMessage($"Latitude must be between {MinLatitude} and {MaxLatitude}.");

        RuleFor(filter => filter.Lng)
            .InclusiveBetween(MinLongitude, MaxLongitude)
            .WithMessage($"Longitude must be between {MinLongitude} and {MaxLongitude}.");

        RuleFor(filter => filter.Radius)
            .Must(radius => radius is >= MinRadiusMeters and <= MaxRadiusMeters)
            .WithMessage($"Radius must be between {MinRadiusMeters} and {MaxRadiusMeters} metres.")
            .When(filter => filter.Radius.HasValue);

        RuleFor(filter => filter.Limit)
            .Must(limit => limit is >= MinLimit and <= MaxLimit)
            .WithMessage($"Limit must be between {MinLimit} and {MaxLimit}.")
            .When(filter => filter.Limit.HasValue);

        RuleFor(filter => filter.Niche)
            .NotEmpty().WithMessage("Niche must not be empty when provided.")
            .MaximumLength(MaxNicheLength).WithMessage($"Niche must be at most {MaxNicheLength} characters.")
            .When(filter => filter.Niche is not null);
    }
}
