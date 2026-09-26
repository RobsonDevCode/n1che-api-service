namespace N1che.Domain.Models.Routes;

/// <summary>One turn-by-turn step of a leg, as the navigation engine drives off it.</summary>
public record RouteStepModel
{
    public required RouteWalkModel Walk { get; init; }

    /// <summary>The instruction to announce, empty when the step carries none.</summary>
    public required string Instruction { get; init; }

    public required string Maneuver { get; init; }
}
