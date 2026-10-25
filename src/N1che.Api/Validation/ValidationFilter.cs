using FluentValidation;

namespace N1che.Api.Validation;

internal sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        var target = context.Arguments.OfType<T>().FirstOrDefault();

        if (validator is null || target is null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(target, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            return TypedResults.ValidationProblem(result.ToDictionary());
        }

        return await next(context);
    }
}
