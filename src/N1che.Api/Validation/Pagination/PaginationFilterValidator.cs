using FluentValidation;
using N1che.Contracts.Filters.Pagination;

namespace N1che.Api.Validation.Pagination;

internal sealed class PaginationFilterValidator : AbstractValidator<PaginationFilter>
{
    private const int MinPage = 1;
    private const int MinSize = 1;
    private const int MaxSize = 100;

    public PaginationFilterValidator()
    {
        RuleFor(filter => filter.Page)
            .GreaterThanOrEqualTo(MinPage)
            .WithMessage($"Page must be at least {MinPage}.");

        RuleFor(filter => filter.Size)
            .InclusiveBetween(MinSize, MaxSize)
            .WithMessage($"Size must be between {MinSize} and {MaxSize}.");
    }
}
