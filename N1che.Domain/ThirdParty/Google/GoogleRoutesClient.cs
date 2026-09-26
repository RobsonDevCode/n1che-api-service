using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Routes;
using N1che.Domain.ThirdParty.Google.Extensions;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.ThirdParty.Google;

public sealed class GoogleRoutesClient : IGoogleRoutesClient
{
    private const string FieldMaskHeader = "X-Goog-FieldMask";

    private const string ComputeRoutesPath = "directions/v2:computeRoutes";

    // Google bills per field group, so the mask asks for exactly what the panels and the nav engine
    // render: the route's geometry, each leg's totals, and the turn-by-turn steps inside them.
    private static readonly string ComputeRoutesFieldMask = string.Join(',',
    [
        "routes.distanceMeters",
        "routes.duration",
        "routes.polyline",
        "routes.legs.distanceMeters",
        "routes.legs.duration",
        "routes.legs.polyline",
        "routes.legs.steps.distanceMeters",
        "routes.legs.steps.staticDuration",
        "routes.legs.steps.polyline",
        "routes.legs.steps.navigationInstruction"
    ]);

    private readonly HttpClient _httpClient;
    private readonly ILogger<GoogleRoutesClient> _logger;

    public GoogleRoutesClient(HttpClient httpClient, ILogger<GoogleRoutesClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<RouteGeometryModel?> ComputeWalkingRoute(
        IReadOnlyCollection<CoordinateModel> waypoints, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, ComputeRoutesPath)
        {
            Content = JsonContent.Create(waypoints.ToGoogleRequest())
        };
        request.Headers.Add(FieldMaskHeader, ComputeRoutesFieldMask);

        using var response = await _httpClient.SendAsync(request, cancellationToken);

        // The stops were read from our own store and the waypoints built from them, so nothing the
        // caller sent is left for Google to reject — a failure here is Google's.
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(cancellationToken);

            _logger.LogError("Google Routes answered {StatusCode} for {WaypointCount} waypoints: {Error}",
                (int)response.StatusCode, waypoints.Count, error);

            throw new GoogleRoutesException();
        }

        var routes = await response.Content.ReadFromJsonAsync<ComputeRoutesResponse>(cancellationToken);

        return routes?.ToDomainModel();
    }
}
