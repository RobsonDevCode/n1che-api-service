namespace N1che.Domain.Models.Niches;

public record NicheModel
{
    public required string Id { get; init; }

    public required string Label { get; init; }

    public required string SubLabel { get; init; }

    public required string Description { get; init; }
}
