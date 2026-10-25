using N1che.Domain.Interfaces.Persistence.Readers.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;
using N1che.Domain.Models.Pagination;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Services.Reviews;

public sealed class ReviewRetrievalService : IReviewRetrievalService
{
    private readonly IReviewsReader _reviewsReader;

    public ReviewRetrievalService(IReviewsReader reviewsReader)
    {
        _reviewsReader = reviewsReader;
    }

    public Task<PaginationModel<ReviewModel>> GetPageByShopIdAsync(Guid shopId, string niche, PaginationDetailsModel pagination, CancellationToken cancellationToken)
    {
        return _reviewsReader.GetPageByShopId(shopId, niche, pagination, cancellationToken);
    }
}
