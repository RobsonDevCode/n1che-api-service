using System.Net;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace N1che.ServiceTests.Infrastructure.Google;

// Stands in for places.googleapis.com: the app is pointed at this server through
// GooglePlaces__BaseUrl, so the real GooglePlacesClient makes real HTTP calls. Scenarios stub the
// place ids they own, and an unstubbed id gets WireMock's 404 — the same answer Google gives for a
// place it has no record of.
internal static class GooglePlacesMock
{
    internal const string ApiKey = "test-google-places-key";
    internal const string OperationalStatus = "OPERATIONAL";
    internal const string PermanentlyClosedStatus = "CLOSED_PERMANENTLY";

    // The key and field mask are part of the match, so a client that sends the wrong ones gets a 404
    // instead of a stubbed place — every scenario then fails, not just one that thinks to assert it.
    private const string ApiKeyHeader = "X-Goog-Api-Key";
    private const string FieldMaskHeader = "X-Goog-FieldMask";

    private const string PlaceDetailsFieldMask =
        "businessStatus,displayName,formattedAddress,location,regularOpeningHours";

    private const string LanguageCode = "en";

    private static WireMockServer? s_server;

    private static WireMockServer Server =>
        s_server ?? throw new InvalidOperationException("Google Places mock has not been started");

    internal static string Start()
    {
        s_server = WireMockServer.Start();
        return s_server.Url!;
    }

    internal static void Stop() => s_server?.Stop();

    internal static void ReturnsPlaceDetails(GooglePlace place)
    {
        Server
            .Given(RequestFor(place.GooglePlaceId))
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new
                {
                    businessStatus = place.BusinessStatus,
                    displayName = new { text = place.Name, languageCode = LanguageCode },
                    formattedAddress = place.Address,
                    location = new { latitude = place.Latitude, longitude = place.Longitude },
                    regularOpeningHours = place.Periods.Count == 0
                        ? null
                        : new { periods = place.Periods.Select(period => period.ToGooglePeriod()).ToArray() }
                }));
    }

    internal static void AnswersWith(string googlePlaceId, HttpStatusCode statusCode)
    {
        Server
            .Given(RequestFor(googlePlaceId))
            .RespondWith(Response.Create().WithStatusCode(statusCode));
    }

    private static IRequestBuilder RequestFor(string googlePlaceId) =>
        Request.Create()
            .WithPath($"/v1/places/{googlePlaceId}")
            .UsingGet()
            .WithHeader(ApiKeyHeader, ApiKey)
            .WithHeader(FieldMaskHeader, PlaceDetailsFieldMask);
}
