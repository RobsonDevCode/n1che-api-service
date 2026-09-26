namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>
/// One step of a leg. Its duration is <c>staticDuration</c> — the walk without traffic, which is all a
/// walking step has — and every member is optional because Google omits what the field mask did not
/// ask for or what it holds no value for.
/// </summary>
public sealed record RouteStepResponse
{
    public double? DistanceMeters { get; init; }

    public string? StaticDuration { get; init; }

    public RoutePolylineResponse? Polyline { get; init; }

    public RouteNavigationInstructionResponse? NavigationInstruction { get; init; }
}
