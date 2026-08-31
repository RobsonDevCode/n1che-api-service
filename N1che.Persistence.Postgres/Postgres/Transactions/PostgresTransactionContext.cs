using Npgsql;

namespace N1che.Persistence.Postgres.Postgres.Transactions;

public sealed class PostgresTransactionContext
{
    public NpgsqlConnection? Connection { get; set; }
    public NpgsqlTransaction? Transaction { get; set; }
}
