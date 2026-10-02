using N1che.Contracts.Response.Routes;

namespace N1che.ServiceTests.Infrastructure.Google;

// What Google Routes answers with, staged on the mock. Every distance and duration is drawn fresh, so no
// number a response carries can be one the service invented rather than one Google sent. Durations are
// whole minutes, so the minutes a response rounds the walk to are exact.
internal static class GoogleRoutesApi
{
    private const int SecondsPerMinute = 60;
    private const string Maneuver = "TURN_LEFT";

    internal static GoogleRoute ReturnsWalkThrough(params CoordinateResponse[] waypoints)
    {
        var route = new GoogleRoute
        {
            DistanceMeters = Random.Shared.Meters(),
            DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
            Polyline = waypoints,
            Legs = waypoints.SkipLast(1).Select((waypoint, index) => new GoogleLeg
            {
                DistanceMeters = Random.Shared.Meters(),
                DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
                Polyline = [waypoint, waypoints[index + 1]],
                Steps =
                [
                    new GoogleStep
                    {
                        DistanceMeters = Random.Shared.Meters(),
                        DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
                        Polyline = [waypoint, waypoints[index + 1]],
                        Instruction = $"Walk to stop {index + 1}",
                        Maneuver = Maneuver
                    }
                ]
            }).ToArray()
        };

        GoogleRoutesMock.ReturnsRoute(waypoints, route);
        return route;
    }

    // Google leaves the instruction out of a step that has none, which is not an empty one.
    internal static GoogleRoute ReturnsWalkWithoutInstructionsThrough(params CoordinateResponse[] waypoints)
    {
        var route = new GoogleRoute
        {
            DistanceMeters = Random.Shared.Meters(),
            DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
            Polyline = waypoints,
            Legs =
            [
                new GoogleLeg
                {
                    DistanceMeters = Random.Shared.Meters(),
                    DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
                    Polyline = waypoints,
                    Steps =
                    [
                        new GoogleStep
                        {
                            DistanceMeters = Random.Shared.Meters(),
                            DurationSeconds = Random.Shared.Minutes() * SecondsPerMinute,
                            Polyline = waypoints
                        }
                    ]
                }
            ]
        };

        GoogleRoutesMock.ReturnsRoute(waypoints, route);
        return route;
    }

    internal static void RoutesNothingThrough(params CoordinateResponse[] waypoints) =>
        GoogleRoutesMock.ReturnsNoRoutes(waypoints);

    // Returns the error the client logs when Google turns the request away.
    internal static string AnswersUnsuccessfullyThrough(params CoordinateResponse[] waypoints)
    {
        GoogleRoutesMock.AnswersUnsuccessfully(waypoints);

        return $"Google Routes answered {(int)GoogleRoutesMock.RejectedApiKeyStatus} for {waypoints.Length} waypoints: " +
               GoogleRoutesMock.ErrorBody;
    }
}
