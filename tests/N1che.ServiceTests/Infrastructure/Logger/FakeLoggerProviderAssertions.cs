using AwesomeAssertions;
using AwesomeAssertions.Execution;
using AwesomeAssertions.Primitives;
using Microsoft.Extensions.Logging;

namespace N1che.ServiceTests.Infrastructure.Logger;

public static class FakeLoggerProviderAssertionsExtensions
{
    public static FakeLoggerProviderAssertions Should(this FakeLoggerProvider instance) => new(instance);
}

public class FakeLoggerProviderAssertions(FakeLoggerProvider instance)
    : ReferenceTypeAssertions<FakeLoggerProvider, FakeLoggerProviderAssertions>(instance, AssertionChain.GetOrCreate())
{
    protected override string Identifier => "logger";

    [CustomAssertion]
    public AndConstraint<FakeLoggerProviderAssertions> HaveLogWith(
        LogLevel level,
        string message,
        IDictionary<string, object>? scopes = null,
        string because = "",
        params object[] becauseArgs)
    {
        var found = FindMatches(level, message, scopes).Any();

        CurrentAssertionChain
            .ForCondition(found)
            .BecauseOf(because, becauseArgs)
            .FailWith(
                "Expected {context:logger} to have a log at level {0} with message {1} and scopes {2}{reason}, but no matching log was found. Logs recorded: {3}",
                level, message, scopes ?? new Dictionary<string, object>(), Describe(Subject.Logs));

        return new AndConstraint<FakeLoggerProviderAssertions>(this);
    }

    [CustomAssertion]
    public AndConstraint<FakeLoggerProviderAssertions> NotHaveLogWith(
        LogLevel level,
        string message,
        IDictionary<string, object>? scopes = null,
        string because = "",
        params object[] becauseArgs)
    {
        var matches = FindMatches(level, message, scopes).ToList();

        CurrentAssertionChain
            .ForCondition(matches.Count == 0)
            .BecauseOf(because, becauseArgs)
            .FailWith(
                "Did not expect {context:logger} to have a log at level {0} with message {1} and scopes {2}{reason}, but found {3}.",
                level, message, scopes ?? new Dictionary<string, object>(), Describe(matches));

        return new AndConstraint<FakeLoggerProviderAssertions>(this);
    }

    private IEnumerable<FakeLog> FindMatches(LogLevel level, string message, IDictionary<string, object>? scopes)
    {
        var matches = Subject.Logs.Where(log => log.LogLevel == level && log.Message == message);

        if (scopes is not null)
        {
            matches = matches.Where(log => scopes.All(scope =>
                log.Scopes.TryGetValue(scope.Key, out var value) && Equals(value, scope.Value)));
        }

        return matches;
    }

    private static string Describe(IEnumerable<FakeLog> logs) =>
        string.Join(", ", logs.Select(log => $"[{log.LogLevel}] \"{log.Message}\""));
}
