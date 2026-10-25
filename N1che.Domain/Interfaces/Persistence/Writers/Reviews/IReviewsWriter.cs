using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Persistence.Writers.Reviews;

/// <summary>Writes reviews to the store.</summary>
public interface IReviewsWriter
{
    /// <summary>Creates the review and returns it as stored.</summary>
    Task<ReviewModel> Create(CreateReviewRequestModel review, CancellationToken cancellationToken);
}
