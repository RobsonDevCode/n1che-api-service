using N1che.Contracts.Requests.Routes;
using N1che.Domain.Models.Routes;
using N1che.Domain.Models.Users;

namespace N1che.Api.Extensions.Routes;

public static class CreateRouteRequestExtensions
{
    public static CreateRouteModel ToDomainModel(this CreateRouteRequest request, UserModel createdBy) => new()
    {
        Name = request.Name,
        Tag = request.Tag,
        Niche = request.Niche,
        StopIds = request.Stops.ToArray(),
        Polyline = request.Polyline.Select(coordinate => coordinate.ToDomainModel()).ToArray(),
        DistanceMeters = request.DistanceMeters,
        TotalMinutes = request.TotalMinutes,
        CreatedByUserId = createdBy.Id,
        CreatedByUsername = createdBy.Username,
    };
}
