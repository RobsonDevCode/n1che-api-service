namespace N1che.Domain.Extensions;

/// <summary>Helpers for deriving query values from a point in time.</summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// The day of week as a zero-based index where 0 is Sunday, matching both
    /// <see cref="System.DayOfWeek"/> and the value Postgres <c>EXTRACT(DOW)</c> produces.
    /// </summary>
    public static int ToDayOfWeekIndex(this DateTime instant) => (int)instant.DayOfWeek;
}
