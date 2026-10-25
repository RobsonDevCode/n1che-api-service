using FluentValidation;
using N1che.Contracts.Filters.Places;

namespace N1che.Api.Validation.Places;

internal sealed class PlacePhotoMediaFilterValidator : AbstractValidator<PlacePhotoMediaFilter>
{
    private const int MinWidth = 1;
    private const int MaxWidth = 4800;

    // The ids are placed into the path of the call to Google, so only the characters Google's ids are
    // made of can pass. \z, not $, which would let a trailing newline through.
    private const string GoogleIdPattern = @"^[A-Za-z0-9_-]+\z";

    public PlacePhotoMediaFilterValidator()
    {
        RuleFor(filter => filter.PlaceId)
            .Matches(GoogleIdPattern)
            .WithMessage("PlaceId must contain only letters, digits, '-' and '_'.");

        RuleFor(filter => filter.PhotoId)
            .Matches(GoogleIdPattern)
            .WithMessage("PhotoId must contain only letters, digits, '-' and '_'.");

        RuleFor(filter => filter.MaxWidth)
            .InclusiveBetween(MinWidth, MaxWidth)
            .WithMessage($"MaxWidth must be between {MinWidth} and {MaxWidth}.");
    }
}
