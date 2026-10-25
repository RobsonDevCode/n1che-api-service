using AwesomeAssertions;
using N1che.Domain.Models.Routes;
using N1che.Domain.ThirdParty.Google.Extensions;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.UnitTests.ThirdParty.Google.Extensions;

public class ComputeRoutesResponseExtensionsTests
{
    public static TheoryData<ComputeRoutesResponse> EmptyResponses =>
    [
        new ComputeRoutesResponse { Routes = null },
        new ComputeRoutesResponse { Routes = [] },
    ];

    [Fact]
    public void ToDomainModel_Returns_Route_With_Latitude_Before_Longitude_When_Google_Returns_A_Complete_Route()
    {
        var response = new ComputeRoutesResponse
        {
            Routes =
            [
                new RouteGeometryResponse
                {
                    DistanceMeters = 700,
                    Duration = "540s",
                    Polyline = LineThrough([-0.0922, 51.5155], [-0.0857, 51.5117]),
                    Legs =
                    [
                        new RouteLegResponse
                        {
                            DistanceMeters = 700,
                            Duration = "540s",
                            Polyline = LineThrough([-0.0922, 51.5155], [-0.0857, 51.5117]),
                            Steps =
                            [
                                new RouteStepResponse
                                {
                                    DistanceMeters = 700,
                                    StaticDuration = "540s",
                                    Polyline = LineThrough([-0.0922, 51.5155], [-0.0857, 51.5117]),
                                    NavigationInstruction = new RouteNavigationInstructionResponse
                                    {
                                        Instructions = "Head south-east on Brick Lane",
                                        Maneuver = "DEPART",
                                    },
                                },
                            ],
                        },
                    ],
                },
            ],
        };

        var polyline = new[]
        {
            new CoordinateModel { Latitude = 51.5155, Longitude = -0.0922 },
            new CoordinateModel { Latitude = 51.5117, Longitude = -0.0857 },
        };
        var walk = new RouteWalkModel { DistanceMeters = 700, DurationSeconds = 540, Polyline = polyline };

        response.ToDomainModel().Should().BeEquivalentTo(new RouteGeometryModel
        {
            Walk = walk,
            Legs =
            [
                new RouteLegModel
                {
                    Walk = walk,
                    Steps = [new RouteStepModel { Walk = walk, Instruction = "Head south-east on Brick Lane", Maneuver = "DEPART" }],
                },
            ],
        }, options => options.WithStrictOrdering());
    }

    [Theory]
    [MemberData(nameof(EmptyResponses))]
    public void ToDomainModel_Returns_Null_When_Google_Returns_No_Route(ComputeRoutesResponse response)
    {
        response.ToDomainModel().Should().BeNull();
    }

    [Theory]
    [InlineData("620s", 620)]
    [InlineData("12.6s", 13)]
    [InlineData("12.5s", 13)]
    [InlineData("13.5s", 14)]
    [InlineData("12.4s", 12)]
    [InlineData("0s", 0)]
    [InlineData("soon", 0)]
    [InlineData(null, 0)]
    public void ToDomainModel_Returns_Whole_Seconds_When_Google_Returns_A_Protobuf_Duration(
        string? duration, int expectedSeconds)
    {
        var route = new ComputeRoutesResponse { Routes = [new RouteGeometryResponse { Duration = duration }] }
            .ToDomainModel();

        route.Should().NotBeNull();
        route.Walk.DurationSeconds.Should().Be(expectedSeconds);
    }

    [Fact]
    public void ToDomainModel_Returns_Only_Whole_Points_When_A_Polyline_Position_Has_Fewer_Than_Two_Values()
    {
        var route = new ComputeRoutesResponse
        {
            Routes = [new RouteGeometryResponse { Polyline = LineThrough([-0.0922, 51.5155], [-0.0890], [], [-0.0857, 51.5117, 14.2]) }],
        }.ToDomainModel();

        route.Should().NotBeNull();
        route.Walk.Polyline.Should().BeEquivalentTo(new[]
        {
            new CoordinateModel { Latitude = 51.5155, Longitude = -0.0922 },
            new CoordinateModel { Latitude = 51.5117, Longitude = -0.0857 },
        }, options => options.WithStrictOrdering());
    }

    [Fact]
    public void ToDomainModel_Returns_Defaults_When_Google_Omits_Optional_Fields()
    {
        var response = new ComputeRoutesResponse
        {
            Routes = [new RouteGeometryResponse { Legs = [new RouteLegResponse { Steps = [new RouteStepResponse()] }] }],
        };

        var emptyWalk = new RouteWalkModel { DistanceMeters = 0, DurationSeconds = 0, Polyline = [] };

        response.ToDomainModel().Should().BeEquivalentTo(new RouteGeometryModel
        {
            Walk = emptyWalk,
            Legs =
            [
                new RouteLegModel
                {
                    Walk = emptyWalk,
                    Steps = [new RouteStepModel { Walk = emptyWalk, Instruction = "", Maneuver = "unknown" }],
                },
            ],
        });
    }

    private static RoutePolylineResponse LineThrough(params double[][] positions) => new()
    {
        GeoJsonLinestring = new GeoJsonLineStringResponse { Coordinates = positions },
    };
}
