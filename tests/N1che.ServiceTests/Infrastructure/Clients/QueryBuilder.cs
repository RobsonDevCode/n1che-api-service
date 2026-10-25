using System.Globalization;

namespace N1che.ServiceTests.Infrastructure.Clients;

public sealed class QueryBuilder
{
    private readonly List<string> _parameters = [];

    public QueryBuilder Add(string key, object? value)
    {
        if (value is null)
        {
            return this;
        }

        var formatted = value is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : value.ToString();

        _parameters.Add($"{key}={Uri.EscapeDataString(formatted!)}");
        return this;
    }

    public string Build() => _parameters.Count == 0 ? string.Empty : $"?{string.Join('&', _parameters)}";
}
