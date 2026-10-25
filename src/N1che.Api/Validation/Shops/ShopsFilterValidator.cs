using FluentValidation;
using N1che.Contracts.Filters.Shops;

namespace N1che.Api.Validation.Shops;

internal sealed class ShopsFilterValidator : AbstractValidator<ShopsFilter>
{
    public ShopsFilterValidator()
    {
        RuleFor(filter => filter.Niche)
            .Must(niches => niches!.All(niche => !string.IsNullOrWhiteSpace(niche)))
            .WithMessage("Niche must not be empty when provided.")
            .When(filter => filter.Niche is not null);
    }
}
