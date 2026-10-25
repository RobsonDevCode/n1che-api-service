using AwesomeAssertions;
using N1che.Domain.Models.Places;
using N1che.Domain.Models.Shops;
using N1che.Domain.ThirdParty.Google.Extensions;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.UnitTests.ThirdParty.Google.Extensions;

public class PlaceDetailsResponseExtensionsTests
{
    private const int Monday = 1;
    private const int Tuesday = 2;
    private const int Friday = 5;
    private const int Saturday = 6;

    private static readonly PlaceDetailsResponse CompleteResponse = new()
    {
        BusinessStatus = "OPERATIONAL",
        DisplayName = new PlaceDisplayNameResponse { Text = "Void & Velvet" },
        FormattedAddress = "12 Brick Lane, London E1 6RF",
        Location = new PlaceLocationResponse { Latitude = 51.5205, Longitude = -0.0716 },
        RegularOpeningHours = new PlaceOpeningHoursResponse
        {
            Periods = [new PlacePeriodResponse { Open = PointAt(Monday, 10, 0), Close = PointAt(Monday, 18, 30) }],
        },
    };

    public static TheoryData<PlaceDetailsResponse> IncompleteResponses =>
    [
        CompleteResponse with { DisplayName = null },
        CompleteResponse with { DisplayName = new PlaceDisplayNameResponse { Text = " " } },
        CompleteResponse with { FormattedAddress = null },
        CompleteResponse with { FormattedAddress = "" },
        CompleteResponse with { Location = null },
    ];

    [Fact]
    public void ToDomainModel_Returns_Place_When_Google_Returns_A_Complete_Place()
    {
        var place = CompleteResponse.ToDomainModel();

        place.Should().BeEquivalentTo(new PlaceDetailsModel
        {
            Name = "Void & Velvet",
            Address = "12 Brick Lane, London E1 6RF",
            Latitude = 51.5205,
            Longitude = -0.0716,
            IsPermanentlyClosed = false,
            OpeningHours =
            [
                new ShopHoursModel { DayOfWeek = Monday, OpenTime = new TimeOnly(10, 0), CloseTime = new TimeOnly(18, 30) },
            ],
        });
    }

    [Theory]
    [MemberData(nameof(IncompleteResponses))]
    public void ToDomainModel_Returns_Null_When_Google_Has_No_Name_Address_Or_Location(PlaceDetailsResponse response)
    {
        response.ToDomainModel().Should().BeNull();
    }

    [Theory]
    [InlineData("CLOSED_PERMANENTLY", true)]
    [InlineData("CLOSED_TEMPORARILY", false)]
    [InlineData("OPERATIONAL", false)]
    [InlineData(null, false)]
    public void ToDomainModel_Returns_Permanently_Closed_Only_When_Google_Says_Closed_Permanently(
        string? businessStatus, bool expectedPermanentlyClosed)
    {
        var place = (CompleteResponse with { BusinessStatus = businessStatus }).ToDomainModel();

        place.Should().NotBeNull();
        place.IsPermanentlyClosed.Should().Be(expectedPermanentlyClosed);
    }

    [Fact]
    public void ToDomainModel_Returns_Earliest_Open_To_Latest_Close_When_A_Day_Has_A_Break()
    {
        var place = (CompleteResponse with
        {
            RegularOpeningHours = new PlaceOpeningHoursResponse
            {
                Periods =
                [
                    new PlacePeriodResponse { Open = PointAt(Monday, 14, 0), Close = PointAt(Monday, 18, 0) },
                    new PlacePeriodResponse { Open = PointAt(Tuesday, 11, 0), Close = PointAt(Tuesday, 17, 0) },
                    new PlacePeriodResponse { Open = PointAt(Monday, 9, 30), Close = PointAt(Monday, 12, 30) },
                ],
            },
        }).ToDomainModel();

        place.Should().NotBeNull();
        place.OpeningHours.Should().BeEquivalentTo(new[]
        {
            new ShopHoursModel { DayOfWeek = Monday, OpenTime = new TimeOnly(9, 30), CloseTime = new TimeOnly(18, 0) },
            new ShopHoursModel { DayOfWeek = Tuesday, OpenTime = new TimeOnly(11, 0), CloseTime = new TimeOnly(17, 0) },
        });
    }

    [Fact]
    public void ToDomainModel_Returns_Close_At_End_Of_Day_When_A_Period_Closes_After_Midnight()
    {
        var place = (CompleteResponse with
        {
            RegularOpeningHours = new PlaceOpeningHoursResponse
            {
                Periods = [new PlacePeriodResponse { Open = PointAt(Friday, 20, 0), Close = PointAt(Saturday, 2, 0) }],
            },
        }).ToDomainModel();

        place.Should().NotBeNull();
        place.OpeningHours.Should().BeEquivalentTo(new[]
        {
            new ShopHoursModel { DayOfWeek = Friday, OpenTime = new TimeOnly(20, 0), CloseTime = new TimeOnly(23, 59, 59) },
        });
    }

    [Fact]
    public void ToDomainModel_Returns_No_Hours_For_A_Period_When_Google_Gives_It_No_Close()
    {
        var place = (CompleteResponse with
        {
            RegularOpeningHours = new PlaceOpeningHoursResponse
            {
                Periods =
                [
                    new PlacePeriodResponse { Open = PointAt(Monday, 0, 0) },
                    new PlacePeriodResponse { Open = PointAt(Tuesday, 10, 0), Close = PointAt(Tuesday, 16, 0) },
                ],
            },
        }).ToDomainModel();

        place.Should().NotBeNull();
        place.OpeningHours.Should().BeEquivalentTo(new[]
        {
            new ShopHoursModel { DayOfWeek = Tuesday, OpenTime = new TimeOnly(10, 0), CloseTime = new TimeOnly(16, 0) },
        });
    }

    [Fact]
    public void ToDomainModel_Returns_No_Hours_When_Google_Has_No_Opening_Hours()
    {
        var place = (CompleteResponse with { RegularOpeningHours = null }).ToDomainModel();

        place.Should().NotBeNull();
        place.OpeningHours.Should().BeEmpty();
    }

    private static PlacePeriodPointResponse PointAt(int day, int hour, int minute) => new()
    {
        Day = day,
        Hour = hour,
        Minute = minute,
    };
}
