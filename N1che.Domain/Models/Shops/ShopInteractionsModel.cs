namespace N1che.Domain.Models.Shops;

/// <summary>A shop's vote count together with one user's interaction state for it.</summary>
public record ShopInteractionsModel
{
    public required int VoteCount { get; init; }

    public required bool Voted { get; init; }

    public required bool Saved { get; init; }
}
