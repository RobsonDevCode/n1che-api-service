using AwesomeAssertions;
using N1che.Domain.Extensions;
using N1che.Domain.Models.Routes;

namespace N1che.Domain.UnitTests.Extensions;

public class CoordinateModelExtensionsTests
{
    private const double ToleranceMeters = 0.01;

    private static readonly CoordinateModel London = new() { Latitude = 51.5074, Longitude = -0.1278 };
    private static readonly CoordinateModel Paris = new() { Latitude = 48.8566, Longitude = 2.3522 };

    [Theory]
    [InlineData(0, 0, 1, 0, 111_194.93)]
    [InlineData(0, 0, 0, 1, 111_194.93)]
    [InlineData(0, 0, 0, 180, 20_015_086.80)]
    [InlineData(51.5074, -0.1278, 48.8566, 2.3522, 343_556.06)]
    public void DistanceMetersTo_Returns_Great_Circle_Distance_When_Points_Differ(
        double fromLatitude, double fromLongitude, double toLatitude, double toLongitude, double expectedMeters)
    {
        var from = new CoordinateModel { Latitude = fromLatitude, Longitude = fromLongitude };
        var to = new CoordinateModel { Latitude = toLatitude, Longitude = toLongitude };

        from.DistanceMetersTo(to).Should().BeApproximately(expectedMeters, ToleranceMeters);
    }

    [Fact]
    public void DistanceMetersTo_Returns_Zero_When_Points_Are_The_Same()
    {
        London.DistanceMetersTo(London).Should().Be(0);
    }

    [Fact]
    public void DistanceMetersTo_Returns_The_Same_Distance_When_Measured_In_Either_Direction()
    {
        London.DistanceMetersTo(Paris).Should().BeApproximately(Paris.DistanceMetersTo(London), ToleranceMeters);
    }
}
