namespace N1che.Domain.Exceptions;

public sealed class InvalidRequestException(string entityType, params object[] parameters) : Exception
{
    private const string NicheFormat = "Niche {0} is not recognised.";
    private const string PlaceFormat = "Google place {0} cannot be added.";
    private const string RouteFormat = "No walking route runs through the requested stops.";
    private const string DefaultMessage = "The request could not be processed.";

    private string Format { get; } = entityType switch
    {
        EntityTypes.Niche => NicheFormat,
        EntityTypes.Place => PlaceFormat,
        EntityTypes.Route => RouteFormat,
        _ => DefaultMessage
    };

    private object[] Parameters { get; } = parameters;

    public override string Message => string.Format(Format, Parameters);
}
