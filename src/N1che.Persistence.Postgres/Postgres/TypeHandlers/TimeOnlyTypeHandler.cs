using System.Data;
using Dapper;

namespace N1che.Persistence.Postgres.Postgres.TypeHandlers;

/// <summary>
/// Dapper has no built-in DbType for <see cref="TimeOnly"/>, so every <c>time</c> parameter would
/// otherwise have to be converted at the call site. Registered once, this sends it as an offset from
/// midnight and reads it back whichever way Npgsql hands it over.
/// </summary>
public sealed class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value.ToTimeSpan();
    }

    public override TimeOnly Parse(object value) => value switch
    {
        TimeOnly time => time,
        TimeSpan offsetFromMidnight => TimeOnly.FromTimeSpan(offsetFromMidnight),
        _ => TimeOnly.Parse((string)value)
    };
}
