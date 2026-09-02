namespace N1che.Domain.Models.Shops;

public record ShopModel
{
    public required string Id { get; init; }

    public required string GooglePlaceId { get; init; }

    public required string Name { get; init; }

    public required IReadOnlyCollection<string> Niches { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required int VoteCount { get; init; }

    public required string PlaceStatus { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string AddedByUserId { get; init; }

    public required string AddedByUsername { get; init; }

    public string? PhotoUrl { get; init; }
}
