namespace N1che.Domain.Models.Shops;

/// <summary>
/// A request to add a Google place as a shop. The place itself is resolved from Google, so the
/// request carries only its identifier and what the user chose.
/// </summary>
public record CreateShopRequestModel
{
    public required string GooglePlaceId { get; init; }

    public required IReadOnlyCollection<string> Niches { get; init; }

    public required string AddedByUserId { get; init; }

    public required string AddedByUsername { get; init; }
}
