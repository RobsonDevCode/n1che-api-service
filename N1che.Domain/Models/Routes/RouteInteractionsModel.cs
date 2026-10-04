namespace N1che.Domain.Models.Routes;

/// <summary>A route's vote count together with one user's interaction state for it.</summary>
public record RouteInteractionsModel
{
    public required int VoteCount { get; init; }

    public required bool Voted { get; init; }
}
