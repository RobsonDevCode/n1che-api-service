namespace N1che.Persistence.Postgres.Postgres.Entities.Shops;

public sealed record ShopEntity
{
    public required Guid Id { get; init; }

    public required string GooglePlaceId { get; init; }

    public required string Name { get; init; }

    public required string[] Niches { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required int VoteCount { get; init; }

    public required string PlaceStatus { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required string AddedByUserId { get; init; }

    public required string AddedByUsername { get; init; }
}
