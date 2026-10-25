namespace N1che.ServiceTests.Infrastructure.Google;

// An author Google credits for a photo.
internal sealed record GoogleAuthorAttribution
{
    public required string? DisplayName { get; init; }

    public required string? Uri { get; init; }

    public required string? PhotoUri { get; init; }
}
