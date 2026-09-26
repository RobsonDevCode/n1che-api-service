namespace N1che.Domain.Models.Routes;

/// <summary>
/// The two shapes a route can take: a walk from where the user is standing through the stops in
/// order, or a loop that returns to the first stop.
/// </summary>
public static class RouteModes
{
    public const string You = "you";

    public const string Loop = "loop";
}
