namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>A viewport, given by its south-west (<c>low</c>) and north-east (<c>high</c>) corners.</summary>
public sealed record PlaceRectangleRequest
{
    public required PlaceLocationRequest Low { get; init; }

    public required PlaceLocationRequest High { get; init; }
}
