namespace N1che.ServiceTests.Infrastructure.Google;

// One of Google's opening-hours periods. Two periods can share a day, which is how Google expresses
// a lunch break, and a close earlier than its open is a place trading into the following day.
internal sealed record TradingPeriod
{
    public required int DayOfWeek { get; init; }

    public required TimeOnly OpenTime { get; init; }

    public required TimeOnly CloseTime { get; init; }

    private int CloseDayOfWeek => CloseTime < OpenTime ? (DayOfWeek + 1) % 7 : DayOfWeek;

    public static TradingPeriod Today(TimeOnly openTime, TimeOnly closeTime) => new()
    {
        DayOfWeek = (int)DateTime.UtcNow.DayOfWeek,
        OpenTime = openTime,
        CloseTime = closeTime
    };

    public object ToGooglePeriod() => new
    {
        open = new { day = DayOfWeek, hour = OpenTime.Hour, minute = OpenTime.Minute },
        close = new { day = CloseDayOfWeek, hour = CloseTime.Hour, minute = CloseTime.Minute }
    };
}
