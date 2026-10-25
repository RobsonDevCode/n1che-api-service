using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Persistence.Readers.Reviews;

/// <summary>Reads reviews from the store.</summary>
public interface IReviewsReader
{
    /// <summary>Gets a page of a shop's reviews by niche.</summary>
    Task<PaginationModel<ReviewModel>> GetPageByShopId(Guid shopId, string niche, PaginationDetailsModel pagination, CancellationToken cancellationToken);
}
