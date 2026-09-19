using FluentValidation;
using N1che.Contracts.Requests.Shops;

namespace N1che.Api.Validation.Shops;

internal sealed class CreateShopRequestValidator : AbstractValidator<CreateShopRequest>
{
    private const int MaxGooglePlaceIdLength = 255;
    private const int MaxNicheLength = 50;

    public CreateShopRequestValidator()
    {
        RuleFor(request => request.GooglePlaceId)
            .NotEmpty().WithMessage("Google place id is required.")
            .MaximumLength(MaxGooglePlaceIdLength)
            .WithMessage($"Google place id must be at most {MaxGooglePlaceIdLength} characters.");

        RuleFor(request => request.Niches)
            .NotEmpty().WithMessage("At least one niche is required.");

        RuleForEach(request => request.Niches)
            .NotEmpty().WithMessage("Niche must not be empty.")
            .MaximumLength(MaxNicheLength).WithMessage($"Niche must be at most {MaxNicheLength} characters.");
    }
}
