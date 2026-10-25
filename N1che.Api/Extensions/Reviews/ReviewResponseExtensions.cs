using N1che.Contracts.Pagination;
using N1che.Contracts.Response.Reviews;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Reviews;

namespace N1che.Api.Extensions.Reviews;

public static class ReviewResponseExtensions
{
    public static PagedResponse<ReviewResponse> ToPagedResponse(this PaginationModel<ReviewModel> pagination) => new()
    {
        Data = pagination.Data.Select(review => review.ToResponse()).ToArray(),
        PaginationDetails = new PaginationDetailsResponse
        {
            TotalCount = pagination.TotalCount,
            CurrentPage = pagination.Page,
            PageSize = pagination.PageSize,
        }
    };

    public static ReviewResponse ToResponse(this ReviewModel review) => new()
    {
        Id = review.Id,
        ShopId = review.ShopId,
        Niche = review.Niche,
        UserId = review.UserId,
        Username = review.Username,
        Text = review.Text,
        CreatedAt = review.CreatedAt,
        UpdatedAt = review.UpdatedAt,
    };
}
