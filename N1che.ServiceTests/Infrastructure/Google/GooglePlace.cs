namespace N1che.ServiceTests.Infrastructure.Google;

// The place Google is standing by to describe. Everything the shop is created from comes from here,
// so a scenario asserts the stored shop against this rather than against anything it posted.
internal sealed record GooglePlace
{
    public required string GooglePlaceId { get; init; }

    public required string BusinessStatus { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public IReadOnlyCollection<TradingPeriod> Periods { get; init; } = [];
}
