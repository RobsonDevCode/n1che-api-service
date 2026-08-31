namespace N1che.Persistence.Postgres.Postgres.Configuration;

public sealed record PostgresOptions
{
    public required string ConnectionString { get; init; }
}
