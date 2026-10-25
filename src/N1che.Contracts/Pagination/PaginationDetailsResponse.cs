namespace N1che.Contracts.Pagination;

public record PaginationDetailsResponse
{
    public required long TotalCount { get; init; }

    public required int PageSize { get; init; }

    public required int CurrentPage { get; init; }
}
