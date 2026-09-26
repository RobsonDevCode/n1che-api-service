using N1che.Contracts.Requests.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class ComputeRouteRequestExtensions
{
    public static ComputeRouteModel ToDomainModel(this ComputeRouteRequest request) => new()
    {
        StopIds = request.Stops.ToArray(),
        Origin = request.Origin is null
            ? null
            : new CoordinateModel
            {
                Latitude = request.Origin.Latitude,
                Longitude = request.Origin.Longitude,
            },
        Mode = request.Mode,
    };
}
