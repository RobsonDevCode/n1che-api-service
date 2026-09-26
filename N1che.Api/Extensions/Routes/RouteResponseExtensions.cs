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

    public static RouteShapeResponse ToResponse(this RouteShapeModel route) => new()
    {
        Stops = route.Stops.Select(stop => stop.ToResponse()).ToArray(),
        Polyline = route.Polyline.Select(coordinate => coordinate.ToResponse()).ToArray(),
        DistanceMeters = route.DistanceMeters,
        TotalMinutes = route.TotalMinutes,
        Mode = route.Mode,
    };

    private static RouteStopResponse ToResponse(this RouteStopModel stop) => new()
    {
        Id = stop.Id,
        Name = stop.Name,
        Address = stop.Address,
        Latitude = stop.Latitude,
        Longitude = stop.Longitude,
        PlaceStatus = stop.PlaceStatus,
        Leg = stop.Leg?.ToResponse(),
    };

    private static RouteLegResponse ToResponse(this RouteLegModel leg) => new()
    {
        DistanceMeters = leg.Walk.DistanceMeters,
        DurationSeconds = leg.Walk.DurationSeconds,
        Polyline = leg.Walk.Polyline.Select(coordinate => coordinate.ToResponse()).ToArray(),
        Steps = leg.Steps.Select(step => step.ToResponse()).ToArray(),
    };

    private static RouteStepResponse ToResponse(this RouteStepModel step) => new()
    {
        DistanceMeters = step.Walk.DistanceMeters,
        DurationSeconds = step.Walk.DurationSeconds,
        Polyline = step.Walk.Polyline.Select(coordinate => coordinate.ToResponse()).ToArray(),
        Instruction = step.Instruction,
        Maneuver = step.Maneuver,
    };

    private static CoordinateResponse ToResponse(this CoordinateModel coordinate) => new()
    {
        Latitude = coordinate.Latitude,
        Longitude = coordinate.Longitude,
    };
}
