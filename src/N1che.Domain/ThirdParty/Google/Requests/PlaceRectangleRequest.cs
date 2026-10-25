namespace N1che.Domain.ThirdParty.Google.Requests;

/// <summary>A viewport, given by its south-west (<c>low</c>) and north-east (<c>high</c>) corners.</summary>
public sealed record PlaceRectangleRequest
{
    public required PointRequest Low { get; init; }

    public required PointRequest High { get; init; }
}
