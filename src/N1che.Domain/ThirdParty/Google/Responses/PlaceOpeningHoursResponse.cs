namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>A place's regular opening hours, absent from the response when Google holds none.</summary>
public sealed record PlaceOpeningHoursResponse
{
    public IReadOnlyCollection<PlacePeriodResponse>? Periods { get; init; }
}
