using Microsoft.Extensions.Logging;
using N1che.ServiceTests.Infrastructure.Logger;

namespace N1che.ServiceTests.CommonSteps;

public static class Logs
{
    public static Task There_Should_Be_A_Log(string message, LogLevel level, Dictionary<string, object> scopes,
        FakeLoggerProvider fakeLogger)
    {
        fakeLogger.Should().HaveLogWith(level, message, scopes);
        return Task.CompletedTask;
    }

    public static Task There_Should_Not_Be_A_Log(string message, LogLevel level, Dictionary<string, object> scopes,
        FakeLoggerProvider fakeLogger)
    {
        fakeLogger.Should().NotHaveLogWith(level, message, scopes);
        return Task.CompletedTask;
    }
}
