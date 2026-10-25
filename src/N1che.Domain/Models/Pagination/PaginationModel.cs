namespace N1che.Domain.Models.Pagination;

public record PaginationModel<T>
{
    public IEnumerable<T> Data { get; init; } = [];

    public required long TotalCount { get; init; }

    public required int Page { get; init; }

    public required int PageSize { get; init; }
}
