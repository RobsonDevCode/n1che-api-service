using N1che.Domain.Models.Routes;

namespace N1che.Domain.Interfaces.Services.Routes;

public interface IRouteRetrievalService
{
    Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearbyAsync(RoutesFilterModel filterModel, CancellationToken cancellationToken);
}
