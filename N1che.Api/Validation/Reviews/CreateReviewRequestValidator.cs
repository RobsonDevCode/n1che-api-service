using FluentValidation;
using N1che.Contracts.Requests.Reviews;

namespace N1che.Api.Validation.Reviews;

internal sealed class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    private const int MaxTextLength = 2000;
    private const int MaxNicheLength = 50;

    public CreateReviewRequestValidator()
    {
        RuleFor(request => request.Text)
            .NotEmpty().WithMessage("Text is required.")
            .MaximumLength(MaxTextLength).WithMessage($"Text must be at most {MaxTextLength} characters.");

        RuleFor(request => request.Niche)
            .NotEmpty().WithMessage("Niche is required.")
            .MaximumLength(MaxNicheLength).WithMessage($"Niche must be at most {MaxNicheLength} characters.");
    }
}
