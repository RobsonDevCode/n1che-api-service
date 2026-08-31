namespace N1che.Domain.Interfaces;

public interface ITransactionScope
{
    Task<T> ExecuteAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken);
}
