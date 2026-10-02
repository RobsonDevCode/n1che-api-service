namespace N1che.Domain.Exceptions;

public sealed class RouteException(string entityType, params object[] parameters) : Exception
{
    private const string RouteFormat = "Route {0} starts more than {1} metres from the requested location.";
    private const string DefaultMessage = "The request could not be processed.";

    private string Format { get; } = entityType switch
    {
        EntityTypes.Route => RouteFormat,
        _ => DefaultMessage
    };

    private object[] Parameters { get; } = parameters;

    public override string Message => string.Format(Format, Parameters);
}
