using N1che.Contracts.Response.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class RouteResponseExtensions
{
    public static IReadOnlyCollection<RouteResponse> ToResponse(this IReadOnlyCollection<RouteModel> routes) =>
        routes.Select(route => route.ToResponse()).ToArray();

    public static RouteResponse ToResponse(this RouteModel route) => new()
    {
        Id = route.Id,
        Name = route.Name,
        Tag = route.Tag,
        Mode = route.Mode,
        Niche = route.Niche,
        CreatedBy = route.CreatedByUsername,
        UserId = route.CreatedByUserId,
        Stops = route.Stops.Select(stop => stop.ToResponse()).ToArray(),
        Polyline = route.Polyline.Select(coordinate => coordinate.ToResponse()).ToArray(),
        DistanceMeters = route.DistanceMeters,
        TotalMinutes = route.TotalMinutes,
        TotalUpvotes = route.VoteCount,
        CreatedAt = route.CreatedAt,
    };

    private static RouteStopResponse ToResponse(this RouteStopModel stop) => new()
    {
        Id = stop.Id,
        Name = stop.Name,
        Address = stop.Address,
        Latitude = stop.Latitude,
        Longitude = stop.Longitude,
        PlaceStatus = stop.PlaceStatus,
    };

    private static CoordinateResponse ToResponse(this CoordinateModel coordinate) => new()
    {
        Latitude = coordinate.Latitude,
        Longitude = coordinate.Longitude,
    };
}
