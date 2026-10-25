using FluentValidation;
using N1che.Contracts.Filters.Reviews;

namespace N1che.Api.Validation.Reviews;

internal sealed class ReviewsFilterValidator : AbstractValidator<ReviewsFilter>
{
    private const int MaxNicheLength = 50;

    public ReviewsFilterValidator()
    {
        RuleFor(filter => filter.Niche)
            .NotEmpty().WithMessage("Niche is required.")
            .MaximumLength(MaxNicheLength).WithMessage($"Niche must be at most {MaxNicheLength} characters.");
    }
}
