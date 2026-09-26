namespace N1che.Domain.ThirdParty.Google.Responses;

/// <summary>
/// The answer to a compute-routes call. Google omits the field entirely when it could route nothing,
/// rather than sending an empty array.
/// </summary>
public sealed record ComputeRoutesResponse
{
    public IReadOnlyCollection<RouteGeometryResponse>? Routes { get; init; }
}
