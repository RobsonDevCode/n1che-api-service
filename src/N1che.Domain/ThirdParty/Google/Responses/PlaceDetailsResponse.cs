namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>A place as returned by Google Places Details, carrying only the requested field mask.</summary>
public sealed record PlaceDetailsResponse
{
    public string? BusinessStatus { get; init; }

    public PlaceDisplayNameResponse? DisplayName { get; init; }

    public string? FormattedAddress { get; init; }

    public PlaceLocationResponse? Location { get; init; }

    public PlaceOpeningHoursResponse? RegularOpeningHours { get; init; }
}
