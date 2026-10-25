using FluentValidation;
using N1che.Contracts.Requests.Reviews;

namespace N1che.Api.Validation.Reviews;

internal sealed class UpdateReviewRequestValidator : AbstractValidator<UpdateReviewRequest>
{
    public UpdateReviewRequestValidator()
    {
        RuleFor(request => request.Text).MustBeReviewText();
    }
}
