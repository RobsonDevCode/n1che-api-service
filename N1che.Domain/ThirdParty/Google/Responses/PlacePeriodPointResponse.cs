namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>When a trading period starts or ends. Day is zero-based from Sunday, as in shop_hours.</summary>
public sealed record PlacePeriodPointResponse
{
    public int Day { get; init; }

    public int Hour { get; init; }

    public int Minute { get; init; }
}
