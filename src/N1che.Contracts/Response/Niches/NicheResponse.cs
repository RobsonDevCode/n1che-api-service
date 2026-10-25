namespace N1che.Contracts.Response.Niches;

public record NicheResponse
{
    public required string Id { get; init; }

    public required string Label { get; init; }

    public required string SubLabel { get; init; }

    public required string Description { get; init; }
}
