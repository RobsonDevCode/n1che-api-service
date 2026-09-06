namespace N1che.Contracts.Pagination;

public record PagedResponse<T>
{
    public IEnumerable<T> Data { get; init; } = [];

    public required PaginationDetailsResponse PaginationDetails { get; init; }
}
