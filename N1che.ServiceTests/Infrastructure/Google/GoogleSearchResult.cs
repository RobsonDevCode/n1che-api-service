namespace N1che.ServiceTests.Infrastructure.Google;

// A place Google is standing by to return from a search. Search hands back a subset of what Details
// holds, so a field Google can omit is nullable here and a scenario can leave it out.
internal sealed record GoogleSearchResult
{
    public required string GooglePlaceId { get; init; }

    public required string Name { get; init; }

    public required string? Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public string? PhotoName { get; init; }
}
