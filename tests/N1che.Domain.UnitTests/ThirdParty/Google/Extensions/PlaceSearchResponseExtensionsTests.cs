using AwesomeAssertions;
using N1che.Domain.Models.Places;
using N1che.Domain.ThirdParty.Google.Extensions;
using N1che.Domain.ThirdParty.Google.Responses;

namespace N1che.Domain.UnitTests.ThirdParty.Google.Extensions;

public class PlaceSearchResponseExtensionsTests
{
    private static readonly PlacePhotoResponse CompletePhoto = new()
    {
        Name = "places/ChIJvoid/photos/AUc7tXfirst",
        GoogleMapsUri = "https://maps.google.com/maps/contrib/101/photos/first",
        AuthorAttributions =
        [
            new PlaceAuthorAttributionResponse
            {
                DisplayName = "Mara Quill",
                Uri = "https://maps.google.com/maps/contrib/101",
                PhotoUri = "https://lh3.googleusercontent.com/a/mara",
            },
        ],
    };

    private static readonly PlaceSummaryResponse CompletePlace = new()
    {
        Id = "ChIJvoid",
        DisplayName = new PlaceDisplayNameResponse { Text = "Void & Velvet" },
        FormattedAddress = "12 Brick Lane, London E1 6RF",
        Location = new PlaceLocationResponse { Latitude = 51.5205, Longitude = -0.0716 },
        Photos = [CompletePhoto],
    };

    private static readonly PlaceModel ExpectedPlace = new()
    {
        GooglePlaceId = "ChIJvoid",
        Name = "Void & Velvet",
        Address = "12 Brick Lane, London E1 6RF",
        Latitude = 51.5205,
        Longitude = -0.0716,
        Photo = new PlacePhotoModel
        {
            Reference = "places/ChIJvoid/photos/AUc7tXfirst",
            GoogleMapsUri = "https://maps.google.com/maps/contrib/101/photos/first",
            AuthorAttributions =
            [
                new PhotoAuthorAttributionModel
                {
                    DisplayName = "Mara Quill",
                    Uri = "https://maps.google.com/maps/contrib/101",
                    PhotoUri = "https://lh3.googleusercontent.com/a/mara",
                },
            ],
        },
    };

    public static TheoryData<PlaceSummaryResponse> IncompletePlaces =>
    [
        CompletePlace with { Id = null },
        CompletePlace with { Id = " " },
        CompletePlace with { DisplayName = null },
        CompletePlace with { DisplayName = new PlaceDisplayNameResponse { Text = "" } },
        CompletePlace with { FormattedAddress = null },
        CompletePlace with { Location = null },
    ];

    public static TheoryData<PlaceSummaryResponse> PlacesWithoutAShowablePhoto =>
    [
        CompletePlace with { Photos = null },
        CompletePlace with { Photos = [] },
        CompletePlace with { Photos = [CompletePhoto with { Name = null }] },
        CompletePlace with { Photos = [CompletePhoto with { GoogleMapsUri = null }] },
        CompletePlace with { Photos = [CompletePhoto with { GoogleMapsUri = " " }] },
    ];

    [Fact]
    public void ToDomainModels_Returns_Places_When_Google_Returns_Complete_Places()
    {
        new PlaceSearchResponse { Places = [CompletePlace] }.ToDomainModels()
            .Should().BeEquivalentTo(new[] { ExpectedPlace });
    }

    [Fact]
    public void ToDomainModels_Returns_Empty_When_Google_Returns_No_Places()
    {
        new PlaceSearchResponse { Places = null }.ToDomainModels().Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(IncompletePlaces))]
    public void ToDomainModels_Returns_Only_The_Complete_Places_When_A_Place_Has_No_Id_Name_Address_Or_Location(
        PlaceSummaryResponse incompletePlace)
    {
        new PlaceSearchResponse { Places = [incompletePlace, CompletePlace] }.ToDomainModels()
            .Should().BeEquivalentTo(new[] { ExpectedPlace });
    }

    [Theory]
    [MemberData(nameof(PlacesWithoutAShowablePhoto))]
    public void ToDomainModels_Returns_Place_Without_A_Photo_When_The_First_Photo_Has_No_Reference_Or_Maps_Link(
        PlaceSummaryResponse place)
    {
        new PlaceSearchResponse { Places = [place] }.ToDomainModels()
            .Should().BeEquivalentTo(new[] { ExpectedPlace with { Photo = null } });
    }

    [Fact]
    public void ToDomainModels_Returns_Only_The_First_Photo_When_Google_Returns_Several()
    {
        var laterPhoto = CompletePhoto with { Name = "places/ChIJvoid/photos/AUc7tXsecond" };

        new PlaceSearchResponse { Places = [CompletePlace with { Photos = [CompletePhoto, laterPhoto] }] }
            .ToDomainModels()
            .Should().BeEquivalentTo(new[] { ExpectedPlace });
    }

    [Fact]
    public void ToDomainModels_Returns_Only_Named_Authors_When_A_Photo_Credits_Authors_Without_A_Name()
    {
        var photo = CompletePhoto with
        {
            AuthorAttributions =
            [
                new PlaceAuthorAttributionResponse { DisplayName = null, Uri = "https://maps.google.com/maps/contrib/102" },
                CompletePhoto.AuthorAttributions!.First(),
                new PlaceAuthorAttributionResponse { DisplayName = " ", Uri = "https://maps.google.com/maps/contrib/103" },
            ],
        };

        new PlaceSearchResponse { Places = [CompletePlace with { Photos = [photo] }] }.ToDomainModels()
            .Should().BeEquivalentTo(new[] { ExpectedPlace });
    }

    [Fact]
    public void ToDomainModels_Returns_Photo_With_No_Authors_When_Google_Credits_None()
    {
        new PlaceSearchResponse { Places = [CompletePlace with { Photos = [CompletePhoto with { AuthorAttributions = null }] }] }
            .ToDomainModels()
            .Should().BeEquivalentTo(new[]
            {
                ExpectedPlace with { Photo = ExpectedPlace.Photo! with { AuthorAttributions = [] } },
            });
    }
}
