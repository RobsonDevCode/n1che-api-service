namespace N1che.Api.Validation.Routes;

/// <summary>The limits a route's stops carry whichever endpoint they arrive at.</summary>
internal static class RouteStopRules
{
    internal const int MinStops = 2;
    internal const int MaxStops = 15;

    internal static readonly string TooManyStops = $"A route can hold at most {MaxStops} stops.";

    internal const string RepeatedStop = "A stop can only appear once in a route.";
}
