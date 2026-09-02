using System.Net;
using AwesomeAssertions;

namespace N1che.ServiceTests.CommonSteps;

public static class HttpResponse
{
    public static Task Status_Code_Is(HttpResponseMessage responseMessage, HttpStatusCode statusCode)
    {
        responseMessage.StatusCode.Should().Be(statusCode);
        return Task.CompletedTask;
    }
}
