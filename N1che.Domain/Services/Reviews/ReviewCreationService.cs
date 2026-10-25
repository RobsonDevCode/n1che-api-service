using N1che.Domain.Interfaces.Persistence.Writers.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Services.Reviews;

public sealed class ReviewCreationService : IReviewCreationService
{
    private readonly IReviewsWriter _reviewsWriter;

    public ReviewCreationService(IReviewsWriter reviewsWriter)
    {
        _reviewsWriter = reviewsWriter;
    }

    public Task<ReviewModel> CreateAsync(CreateReviewRequestModel review, CancellationToken cancellationToken)
    {
        return _reviewsWriter.Create(review, cancellationToken);
    }
}
