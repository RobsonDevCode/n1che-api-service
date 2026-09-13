using N1che.Domain.Interfaces.Persistence.Readers.Routes;
using N1che.Domain.Interfaces.Services.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.Services.Routes;

public sealed class RouteRetrievalService : IRouteRetrievalService
{
    private readonly IRoutesReader _routesReader;

    public RouteRetrievalService(IRoutesReader routesReader)
    {
        _routesReader = routesReader;
    }

    public Task<IReadOnlyCollection<RouteModel>> GetTopRatedNearbyAsync(RoutesFilterModel filterModel, CancellationToken cancellationToken)
    {
        return _routesReader.GetTopRatedNearby(filterModel, cancellationToken);
    }
}
