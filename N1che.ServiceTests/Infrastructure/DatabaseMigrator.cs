using Npgsql;

namespace N1che.ServiceTests.Infrastructure;

public static class DatabaseMigrator
{
    private const string UpMarker = "-- +goose Up";
    private const string DownMarker = "-- +goose Down";

    public static async Task ApplyAsync(string connectionString, CancellationToken cancellationToken = default)
    {
        var migrationsDirectory = Path.Combine(AppContext.BaseDirectory, "Migrations");
        var files = Directory.GetFiles(migrationsDirectory, "*.sql").OrderBy(path => path, StringComparer.Ordinal);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        foreach (var file in files)
        {
            var up = ExtractUpBlock(await File.ReadAllTextAsync(file, cancellationToken));
            if (string.IsNullOrWhiteSpace(up))
            {
                continue;
            }

            await using var command = new NpgsqlCommand(up, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static string ExtractUpBlock(string migration)
    {
        var upStart = migration.IndexOf(UpMarker, StringComparison.Ordinal);
        if (upStart < 0)
        {
            return string.Empty;
        }

        upStart += UpMarker.Length;
        var downStart = migration.IndexOf(DownMarker, upStart, StringComparison.Ordinal);
        var block = downStart < 0 ? migration[upStart..] : migration[upStart..downStart];

        var statements = block.Split('\n').Where(line => !line.TrimStart().StartsWith("-- +goose", StringComparison.Ordinal));
        return string.Join('\n', statements);
    }
}
