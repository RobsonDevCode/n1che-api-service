namespace N1che.Persistence.Postgres.Postgres.Entities.Shops;

public sealed record ShopHoursEntity
{
    public required Guid Id { get; init; }

    public required Guid ShopId { get; init; }

    /// <summary>0 = Sunday, matching Postgres EXTRACT(DOW) and Google's opening-hours periods.</summary>
    public required int DayOfWeek { get; init; }

    public required TimeOnly OpenTime { get; init; }

    public required TimeOnly CloseTime { get; init; }
}
