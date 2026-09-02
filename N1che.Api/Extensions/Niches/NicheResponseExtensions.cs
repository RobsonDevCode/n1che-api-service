using N1che.Contracts.Response.Niches;
using N1che.Domain.Models.Niches;

namespace N1che.Api.Extensions.Niches;

public static class NicheResponseExtensions
{
    public static IReadOnlyCollection<NicheResponse> ToResponse(this IReadOnlyCollection<NicheModel> niches) =>
        niches.Select(niche => niche.ToResponse()).ToArray();

    public static NicheResponse ToResponse(this NicheModel niche) => new()
    {
        Id = niche.Id,
        Label = niche.Label,
        SubLabel = niche.SubLabel,
        Description = niche.Description,
    };
}
