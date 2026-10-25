using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Services.Reviews;

/// <summary>Retrieves reviews for the review endpoints.</summary>
public interface IReviewRetrievalService
{
    /// <summary>Gets a page of a shop's reviews by niche.</summary>
    Task<PaginationModel<ReviewModel>> GetPageByShopIdAsync(Guid shopId, string niche, PaginationDetailsModel pagination, CancellationToken cancellationToken);
}
