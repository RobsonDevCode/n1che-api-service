using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Services.Reviews;

/// <summary>Saves edits users make to their own reviews.</summary>
public interface IReviewUpdateService
{
    /// <summary>Replaces the review's text and returns it as stored.</summary>
    /// <exception cref="Exceptions.NotFoundException">The shop has no review with that id written by the user.</exception>
    Task<ReviewModel> UpdateAsync(UpdateReviewRequestModel review, CancellationToken cancellationToken);
}
