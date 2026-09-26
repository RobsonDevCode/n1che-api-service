namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>What to announce at a step, and the turn it describes.</summary>
public sealed record RouteNavigationInstructionResponse
{
    public string? Instructions { get; init; }

    public string? Maneuver { get; init; }
}
