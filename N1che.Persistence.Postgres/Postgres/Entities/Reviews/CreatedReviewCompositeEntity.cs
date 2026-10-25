namespace N1che.Persistence.Postgres.Postgres.Entities.Reviews;

/// <summary>Whether the shop is listed under the review's niche, joined to the review inserted for it.</summary>
public sealed record CreatedReviewCompositeEntity
{
    public required bool NicheListed { get; init; }

    public ReviewEntity? Review { get; init; }
}
