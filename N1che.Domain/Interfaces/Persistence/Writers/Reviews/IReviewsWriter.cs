using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Persistence.Writers.Reviews;

/// <summary>Writes reviews to the store.</summary>
public interface IReviewsWriter
{
    /// <summary>
    /// Creates the review and returns it as stored. Throws <see cref="Exceptions.NotFoundException"/> when no
    /// shop has that id, and <see cref="Exceptions.InvalidRequestException"/> when the shop is not listed under
    /// the review's niche.
    /// </summary>
    Task<ReviewModel> Create(CreateReviewRequestModel review, CancellationToken cancellationToken);
}
