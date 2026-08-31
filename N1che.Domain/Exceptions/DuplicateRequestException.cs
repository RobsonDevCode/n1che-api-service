namespace N1che.Domain.Exceptions;

public sealed class DuplicateRequestException(string entityType, params object[] parameters) : Exception
{
    private const string ShopFormat = "Shop {0} already exists in the database.";
    private const string RouteFormat = "Route {0} already exists in the database.";
    private const string VoteFormat = "You have already voted on {0}.";
    private const string BookmarkFormat = "{0} is already bookmarked.";
    private const string DefaultMessage = "An item with the same key has already been added.";

    private string Format { get; } = entityType switch
    {
        EntityTypes.Shop => ShopFormat,
        EntityTypes.Route => RouteFormat,
        EntityTypes.Vote => VoteFormat,
        EntityTypes.Bookmark => BookmarkFormat,
        _ => DefaultMessage
    };

    private object[] Parameters { get; } = parameters;

    public override string Message => string.Format(Format, Parameters);
}
