using N1che.Api.Endpoints.Niches;
using N1che.Api.Endpoints.Places;
using N1che.Api.Endpoints.Routes;
using N1che.Api.Endpoints.Shops;

namespace N1che.Api.Endpoints;

internal static class EndpointBuilder
{
    internal static WebApplication MapEndpoints(this WebApplication endpoints)
    {
        endpoints.AddNicheEndpoints();
        endpoints.AddShopEndpoints();
        endpoints.AddRouteEndpoints();
        endpoints.AddPlaceEndpoints();

        return endpoints;
    }
}
