namespace N1che.Persistence.Postgres.Postgres.Entities.Niches;

public sealed record NicheEntity
{
    public required string Id { get; init; }

    public required string Label { get; init; }

    public required string SubLabel { get; init; }

    public required string Description { get; init; }
}
