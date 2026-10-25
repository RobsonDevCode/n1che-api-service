namespace N1che.Domain.Models.Shops;

/// <summary>One day's trading window for a shop.</summary>
public record ShopHoursModel
{
    /// <summary>0 = Sunday, matching Postgres EXTRACT(DOW) and Google's opening-hours periods.</summary>
    public required int DayOfWeek { get; init; }

    public required TimeOnly OpenTime { get; init; }

    public required TimeOnly CloseTime { get; init; }
}
