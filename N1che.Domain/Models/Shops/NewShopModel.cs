namespace N1che.Domain.Models.Shops;

/// <summary>A shop about to be created, as submitted by the user who is adding it.</summary>
public record NewShopModel
{
    public required string GooglePlaceId { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required IReadOnlyCollection<string> Niches { get; init; }

    public required string AddedByUserId { get; init; }

    public required string AddedByUsername { get; init; }
}
