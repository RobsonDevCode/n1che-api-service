namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>A place's name, in the language Google resolved the request to.</summary>
public sealed record PlaceDisplayNameResponse
{
    public string? Text { get; init; }
}
