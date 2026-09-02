namespace N1che.Api.Validation;

internal static class ValidationEndpointExtensions
{
    internal static RouteHandlerBuilder WithValidation<T>(this RouteHandlerBuilder builder) where T : class =>
        builder.AddEndpointFilter<ValidationFilter<T>>();
}
