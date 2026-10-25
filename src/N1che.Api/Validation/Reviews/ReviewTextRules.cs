using FluentValidation;

namespace N1che.Api.Validation.Reviews;

/// <summary>The limits a review's text carries whether it is being written or edited.</summary>
internal static class ReviewTextRules
{
    private const int MaxTextLength = 2000;

    internal static IRuleBuilderOptions<T, string> MustBeReviewText<T>(this IRuleBuilder<T, string> rule) =>
        rule.NotEmpty().WithMessage("Text is required.")
            .MaximumLength(MaxTextLength).WithMessage($"Text must be at most {MaxTextLength} characters.");
}
