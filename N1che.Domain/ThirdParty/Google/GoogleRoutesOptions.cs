namespace N1che.Domain.ThirdParty.Google;

/// <summary>Connection detail for Google Routes; the key comes from configuration, never the caller.</summary>
public sealed record GoogleRoutesOptions
{
    public required string BaseUrl { get; init; }

    public required string ApiKey { get; init; }
}
