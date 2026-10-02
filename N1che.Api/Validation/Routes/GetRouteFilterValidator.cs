using FluentValidation;
using N1che.Contracts.Filters.Routes;

namespace N1che.Api.Validation.Routes;

internal sealed class GetRouteFilterValidator : AbstractValidator<GetRouteFilter>
{
    public GetRouteFilterValidator()
    {
        RuleFor(filter => filter.Lat).MustBeALatitude();

        RuleFor(filter => filter.Lng).MustBeALongitude();
    }
}
