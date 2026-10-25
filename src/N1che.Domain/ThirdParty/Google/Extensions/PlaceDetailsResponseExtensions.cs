using N1che.Domain.Models.Places;
using N1che.Domain.Models.Shops;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google.Extensions;

public static class PlaceDetailsResponseExtensions
{
    private const string PermanentlyClosedStatus = "CLOSED_PERMANENTLY";

    private static readonly TimeOnly EndOfDay = new(23, 59, 59);

    /// <summary>Null when Google holds too little of the place for a shop to be built from it.</summary>
    public static PlaceDetailsModel? ToDomainModel(this PlaceDetailsResponse response)
    {
        var name = response.DisplayName?.Text;
        if (string.IsNullOrWhiteSpace(name)
            || string.IsNullOrWhiteSpace(response.FormattedAddress)
            || response.Location is null)
        {
            return null;
        }

        return new PlaceDetailsModel
        {
            Name = name,
            Address = response.FormattedAddress,
            Latitude = response.Location.Latitude,
            Longitude = response.Location.Longitude,
            IsPermanentlyClosed = string.Equals(response.BusinessStatus, PermanentlyClosedStatus, StringComparison.Ordinal),
            OpeningHours = ToTradingHours(response.RegularOpeningHours),
        };
    }

    // Google splits a day with a break into several periods and gives a round-the-clock place an open
    // with no close, while shop_hours holds one window per day — so each day collapses to its earliest
    // open and latest close, and a period we can't close is dropped.
    private static IReadOnlyCollection<ShopHoursModel> ToTradingHours(PlaceOpeningHoursResponse? openingHours)
    {
        var periods = openingHours?.Periods ?? [];

        return periods
            .Where(period => period.Open is not null && period.Close is not null)
            .GroupBy(period => period.Open!.Day)
            .Select(day => new ShopHoursModel
            {
                DayOfWeek = day.Key,
                OpenTime = day.Min(period => ToTimeOnly(period.Open!)),
                CloseTime = day.Max(ClosingTimeOnTheOpenDay),
            })
            .ToArray();
    }

    // A place trading past midnight closes on the day after it opened, which one day's window cannot
    // hold, so the window runs to the end of the open day and the small hours are given up.
    private static TimeOnly ClosingTimeOnTheOpenDay(PlacePeriodResponse period) =>
        period.Close!.Day == period.Open!.Day ? ToTimeOnly(period.Close) : EndOfDay;

    private static TimeOnly ToTimeOnly(PlacePeriodPointResponse point) => new(point.Hour, point.Minute);
}
