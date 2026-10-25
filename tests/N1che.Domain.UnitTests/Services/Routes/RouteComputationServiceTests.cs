using AwesomeAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.ThirdParty;
using N1che.Domain.Models.Routes;
using N1che.Domain.Models.Shops;
using N1che.Domain.Services.Routes;
using NSubstitute;

namespace N1che.Domain.UnitTests.Services.Routes;

public class RouteComputationServiceTests
{
    private const int WalkDurationSeconds = 1080;
    private const int WalkMinutes = 18;

    private static readonly ShopModel FirstShop = CreateShop("Void & Velvet", 51.5155, -0.0922);
    private static readonly ShopModel SecondShop = CreateShop("Séance Supply", 51.5136, -0.0890);
    private static readonly ShopModel ThirdShop = CreateShop("Harwick & Sons", 51.5117, -0.0857);

    private static readonly CoordinateModel Origin = new() { Latitude = 51.5200, Longitude = -0.0950 };

    private static readonly RouteLegModel FirstLeg = CreateLeg(410);
    private static readonly RouteLegModel SecondLeg = CreateLeg(290);
    private static readonly RouteLegModel ThirdLeg = CreateLeg(350);

    private readonly IShopsReader _shopsReader;
    private readonly IGoogleRoutesClient _googleRoutesClient;
    private readonly RouteComputationService _service;

    public RouteComputationServiceTests()
    {
        _shopsReader = Substitute.For<IShopsReader>();
        _googleRoutesClient = Substitute.For<IGoogleRoutesClient>();
        _service = new RouteComputationService(
            _shopsReader, _googleRoutesClient, NullLogger<RouteComputationService>.Instance);

        // Rows come back out of request order so every test proves the stops follow the requested ids.
        _shopsReader.GetByIds(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns([ThirdShop, FirstShop, SecondShop]);
    }

    [Fact]
    public async Task ComputeAsync_Returns_Each_Leg_On_The_Stop_It_Arrives_At_When_Walking_From_An_Origin()
    {
        var geometry = CreateGeometry(WalkDurationSeconds, FirstLeg, SecondLeg, ThirdLeg);
        _googleRoutesClient.ComputeWalkingRoute(default!, default).ReturnsForAnyArgs(geometry);

        var result = await _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Origin = Origin,
            Mode = RouteModes.You,
        }, CancellationToken.None);

        result.Should().BeEquivalentTo(new RouteShapeModel
        {
            Stops =
            [
                CreateStop(FirstShop, 0, FirstLeg),
                CreateStop(SecondShop, 1, SecondLeg),
                CreateStop(ThirdShop, 2, ThirdLeg),
            ],
            Polyline = geometry.Walk.Polyline,
            DistanceMeters = geometry.Walk.DistanceMeters,
            TotalMinutes = WalkMinutes,
            Mode = RouteModes.You,
        }, options => options.WithStrictOrdering());

        await _googleRoutesClient.Received(1).ComputeWalkingRoute(
            Arg.Is<IReadOnlyCollection<CoordinateModel>>(waypoints => waypoints.SequenceEqual(new[]
            {
                Origin, CoordinatesOf(FirstShop), CoordinatesOf(SecondShop), CoordinatesOf(ThirdShop),
            })),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ComputeAsync_Returns_No_Leg_On_The_First_Stop_When_Walking_Without_An_Origin()
    {
        var geometry = CreateGeometry(WalkDurationSeconds, FirstLeg, SecondLeg);
        _googleRoutesClient.ComputeWalkingRoute(default!, default).ReturnsForAnyArgs(geometry);

        var result = await _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Mode = RouteModes.You,
        }, CancellationToken.None);

        result.Should().BeEquivalentTo(new RouteShapeModel
        {
            Stops =
            [
                CreateStop(FirstShop, 0, null),
                CreateStop(SecondShop, 1, FirstLeg),
                CreateStop(ThirdShop, 2, SecondLeg),
            ],
            Polyline = geometry.Walk.Polyline,
            DistanceMeters = geometry.Walk.DistanceMeters,
            TotalMinutes = WalkMinutes,
            Mode = RouteModes.You,
        }, options => options.WithStrictOrdering());

        await _googleRoutesClient.Received(1).ComputeWalkingRoute(
            Arg.Is<IReadOnlyCollection<CoordinateModel>>(waypoints => waypoints.SequenceEqual(new[]
            {
                CoordinatesOf(FirstShop), CoordinatesOf(SecondShop), CoordinatesOf(ThirdShop),
            })),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ComputeAsync_Returns_The_Last_Leg_On_The_First_Stop_When_Walking_A_Loop()
    {
        var geometry = CreateGeometry(WalkDurationSeconds, FirstLeg, SecondLeg, ThirdLeg);
        _googleRoutesClient.ComputeWalkingRoute(default!, default).ReturnsForAnyArgs(geometry);

        var result = await _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Mode = RouteModes.Loop,
        }, CancellationToken.None);

        result.Should().BeEquivalentTo(new RouteShapeModel
        {
            Stops =
            [
                CreateStop(FirstShop, 0, ThirdLeg),
                CreateStop(SecondShop, 1, FirstLeg),
                CreateStop(ThirdShop, 2, SecondLeg),
            ],
            Polyline = geometry.Walk.Polyline,
            DistanceMeters = geometry.Walk.DistanceMeters,
            TotalMinutes = WalkMinutes,
            Mode = RouteModes.Loop,
        }, options => options.WithStrictOrdering());

        await _googleRoutesClient.Received(1).ComputeWalkingRoute(
            Arg.Is<IReadOnlyCollection<CoordinateModel>>(waypoints => waypoints.SequenceEqual(new[]
            {
                CoordinatesOf(FirstShop), CoordinatesOf(SecondShop), CoordinatesOf(ThirdShop), CoordinatesOf(FirstShop),
            })),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ComputeAsync_Returns_Only_Legs_That_Arrive_At_A_Stop_When_Google_Returns_More_Legs_Than_Stops()
    {
        _googleRoutesClient.ComputeWalkingRoute(default!, default)
            .ReturnsForAnyArgs(CreateGeometry(WalkDurationSeconds, FirstLeg, SecondLeg, ThirdLeg));

        var result = await _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Mode = RouteModes.You,
        }, CancellationToken.None);

        result.Stops.Select(stop => stop.Leg).Should().Equal(null, FirstLeg, SecondLeg);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(29, 0)]
    [InlineData(30, 1)]
    [InlineData(31, 1)]
    [InlineData(150, 3)]
    [InlineData(210, 4)]
    [InlineData(620, 10)]
    [InlineData(1080, 18)]
    public async Task ComputeAsync_Returns_Walk_Rounded_To_The_Nearest_Minute_When_Google_Returns_Seconds(
        int durationSeconds, int expectedMinutes)
    {
        _googleRoutesClient.ComputeWalkingRoute(default!, default)
            .ReturnsForAnyArgs(CreateGeometry(durationSeconds, FirstLeg, SecondLeg));

        var result = await _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Mode = RouteModes.You,
        }, CancellationToken.None);

        result.TotalMinutes.Should().Be(expectedMinutes);
    }

    [Fact]
    public async Task ComputeAsync_Throws_Not_Found_When_A_Stop_Names_No_Shop()
    {
        var unknownShopId = Guid.NewGuid();

        var compute = () => _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, unknownShopId, ThirdShop.Id],
            Mode = RouteModes.You,
        }, CancellationToken.None);

        await compute.Should().ThrowAsync<NotFoundException>().WithMessage($"Shop {unknownShopId} not found");
        await _googleRoutesClient.DidNotReceiveWithAnyArgs().ComputeWalkingRoute(default!, default);
    }

    [Fact]
    public async Task ComputeAsync_Throws_Invalid_Request_When_Google_Finds_No_Walking_Route()
    {
        _googleRoutesClient.ComputeWalkingRoute(default!, default).ReturnsForAnyArgs((RouteGeometryModel?)null);

        var compute = () => _service.ComputeAsync(new ComputeRouteModel
        {
            StopIds = [FirstShop.Id, SecondShop.Id, ThirdShop.Id],
            Mode = RouteModes.You,
        }, CancellationToken.None);

        await compute.Should().ThrowAsync<InvalidRequestException>()
            .WithMessage("No walking route runs through the requested stops.");
    }

    private static ShopModel CreateShop(string name, double latitude, double longitude) => new()
    {
        Id = Guid.NewGuid(),
        GooglePlaceId = $"place-{name}",
        Name = name,
        Niches = ["goth"],
        Address = $"{name}, London",
        Latitude = latitude,
        Longitude = longitude,
        VoteCount = 0,
        PlaceStatus = "open",
        CreatedAt = DateTime.UtcNow,
        AddedByUserId = "11111111-1111-1111-1111-111111111111",
        AddedByUsername = "test-user",
    };

    private static RouteLegModel CreateLeg(double distanceMeters) => new()
    {
        Walk = new RouteWalkModel { DistanceMeters = distanceMeters, DurationSeconds = 300, Polyline = [] },
        Steps = [],
    };

    private static RouteGeometryModel CreateGeometry(int durationSeconds, params RouteLegModel[] legs) => new()
    {
        Walk = new RouteWalkModel
        {
            DistanceMeters = 1050,
            DurationSeconds = durationSeconds,
            Polyline = [CoordinatesOf(FirstShop), CoordinatesOf(ThirdShop)],
        },
        Legs = legs,
    };

    private static RouteStopModel CreateStop(ShopModel shop, int position, RouteLegModel? leg) => new()
    {
        Id = shop.Id,
        Name = shop.Name,
        Address = shop.Address,
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
        PlaceStatus = shop.PlaceStatus,
        Position = position,
        Leg = leg,
    };

    private static CoordinateModel CoordinatesOf(ShopModel shop) => new()
    {
        Latitude = shop.Latitude,
        Longitude = shop.Longitude,
    };
}
