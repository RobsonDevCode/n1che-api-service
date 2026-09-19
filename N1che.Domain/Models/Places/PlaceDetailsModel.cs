using N1che.Domain.Models.Shops;

namespace N1che.Domain.Models.Places;

/// <summary>The Google Places detail N1che needs when a place is added as a shop.</summary>
public record PlaceDetailsModel
{
    public required string Name { get; init; }

    public required string Address { get; init; }

    public required double Latitude { get; init; }

    public required double Longitude { get; init; }

    public required bool IsPermanentlyClosed { get; init; }

    /// <summary>One window per day Google has hours for; empty when it has none for the place.</summary>
    public required IReadOnlyCollection<ShopHoursModel> OpeningHours { get; init; }
}
