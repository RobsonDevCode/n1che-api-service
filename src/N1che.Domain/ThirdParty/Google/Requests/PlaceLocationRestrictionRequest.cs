namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>
/// The region a text search is confined to. Google drops anything outside it, unlike a location bias,
/// which only reorders. Text search restricts to a rectangle and will not take a circle.
/// </summary>
public sealed record PlaceLocationRestrictionRequest
{
    public required PlaceRectangleRequest Rectangle { get; init; }
}
