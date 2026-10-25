using N1che.Domain.Interfaces.Persistence.Writers.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Services.Reviews;

public sealed class ReviewUpdateService : IReviewUpdateService
{
    private readonly IReviewsWriter _reviewsWriter;

    public ReviewUpdateService(IReviewsWriter reviewsWriter)
    {
        _reviewsWriter = reviewsWriter;
    }

    public Task<ReviewModel> UpdateAsync(UpdateReviewRequestModel review, CancellationToken cancellationToken)
    {
        return _reviewsWriter.Update(review, cancellationToken);
    }
}
