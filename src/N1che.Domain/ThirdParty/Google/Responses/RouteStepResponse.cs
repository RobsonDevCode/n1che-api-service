namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>One step of a leg, whose duration is <c>staticDuration</c>, the walk without traffic.</summary>
public sealed record RouteStepResponse
{
    public double? DistanceMeters { get; init; }

    public string? StaticDuration { get; init; }

    public RoutePolylineResponse? Polyline { get; init; }

    public RouteNavigationInstructionResponse? NavigationInstruction { get; init; }
}
