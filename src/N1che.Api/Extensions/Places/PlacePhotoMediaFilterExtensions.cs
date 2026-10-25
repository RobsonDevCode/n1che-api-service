using N1che.Contracts.Filters.Places;
using N1che.Domain.Models.Places;

namespace N1che.Api.Extensions.Places;

public static class PlacePhotoMediaFilterExtensions
{
    public static PlacePhotoMediaFilterModel ToDomainFilter(this PlacePhotoMediaFilter filter) => new()
    {
        Reference = $"places/{filter.PlaceId}/photos/{filter.PhotoId}",
        MaxWidth = filter.MaxWidth,
    };
}
