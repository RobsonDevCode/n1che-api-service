namespace N1che.ServiceTests.Infrastructure.Google;

// A photo Google returns on a searched place. The link and author fields are nullable so a scenario can
// leave out what Google sometimes omits.
internal sealed record GooglePhoto
{
    public required string Name { get; init; }

    public required string? GoogleMapsUri { get; init; }

    public required IReadOnlyCollection<GoogleAuthorAttribution> AuthorAttributions { get; init; }
}
