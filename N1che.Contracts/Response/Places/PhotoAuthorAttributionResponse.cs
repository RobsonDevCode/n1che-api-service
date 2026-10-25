namespace N1che.Contracts.Response.Places;

/// <summary>An author to credit for a photo, using every field present when space allows.</summary>
public record PhotoAuthorAttributionResponse
{
    /// <summary>The author's name.</summary>
    public required string DisplayName { get; init; }

    /// <summary>Link to the author's profile; absent when Google holds none.</summary>
    public string? Uri { get; init; }

    /// <summary>Address of the author's avatar image; absent when Google holds none.</summary>
    public string? PhotoUri { get; init; }
}
