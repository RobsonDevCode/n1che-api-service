using FluentValidation;

namespace N1che.Api.Validation;

/// <summary>
/// The WGS 84 bounds every coordinate the API accepts has to fall inside. These are a property of
/// the coordinate system rather than a per-endpoint limit, so the rule is shared; endpoint-specific
/// caps such as radius or page size stay in the validator that owns them.
/// </summary>
internal static class CoordinateRules
{
    private const double MinLatitude = -90;
    private const double MaxLatitude = 90;
    private const double MinLongitude = -180;
    private const double MaxLongitude = 180;

    internal static IRuleBuilderOptions<T, double> MustBeALatitude<T>(this IRuleBuilder<T, double> rule) =>
        rule.InclusiveBetween(MinLatitude, MaxLatitude)
            .WithMessage($"Latitude must be between {MinLatitude} and {MaxLatitude}.");

    internal static IRuleBuilderOptions<T, double> MustBeALongitude<T>(this IRuleBuilder<T, double> rule) =>
        rule.InclusiveBetween(MinLongitude, MaxLongitude)
            .WithMessage($"Longitude must be between {MinLongitude} and {MaxLongitude}.");
}
