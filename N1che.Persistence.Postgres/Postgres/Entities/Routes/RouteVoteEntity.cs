namespace N1che.Persistence.Postgres.Postgres.Entities.Routes;

/// <summary>One user's vote for one route.</summary>
public sealed record RouteVoteEntity
{
    public required Guid RouteId { get; init; }

    public required string UserId { get; init; }

    public required DateTime CreatedAt { get; init; }
}
