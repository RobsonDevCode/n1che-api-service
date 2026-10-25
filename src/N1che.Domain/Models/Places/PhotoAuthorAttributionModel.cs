namespace N1che.Domain.Models.Places;

/// <summary>The author credited for a place photo.</summary>
public record PhotoAuthorAttributionModel
{
    public required string DisplayName { get; init; }

    /// <summary>The author's profile.</summary>
    public string? Uri { get; init; }

    /// <summary>The author's avatar.</summary>
    public string? PhotoUri { get; init; }
}
