namespace N1che.Domain.Exceptions;

public sealed class NotFoundException(string entityType, params object[] parameters) : Exception
{
    private const string ShopFormat = "Shop {0} not found";
    private const string RouteFormat = "Route {0} not found";
    private const string ReviewFormat = "Review {0} not found";
    private const string NicheFormat = "Niche {0} not found";
    private const string DefaultMessage = "Data not found";

    private string Format { get; } = entityType switch
    {
        EntityTypes.Shop => ShopFormat,
        EntityTypes.Route => RouteFormat,
        EntityTypes.Review => ReviewFormat,
        EntityTypes.Niche => NicheFormat,
        _ => DefaultMessage
    };

    private object[] Parameters { get; } = parameters;

    public override string Message => string.Format(Format, Parameters);
}
