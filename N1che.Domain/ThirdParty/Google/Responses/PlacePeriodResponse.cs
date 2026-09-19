namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>One stretch of trading; a place open around the clock has an open with no close.</summary>
public sealed record PlacePeriodResponse
{
    public PlacePeriodPointResponse? Open { get; init; }

    public PlacePeriodPointResponse? Close { get; init; }
}
