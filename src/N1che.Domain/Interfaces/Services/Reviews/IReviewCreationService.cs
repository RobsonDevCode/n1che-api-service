using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Interfaces.Services.Reviews;

/// <summary>Saves the reviews users write.</summary>
public interface IReviewCreationService
{
    /// <summary>Saves the review and returns it as stored.</summary>
    /// <exception cref="Exceptions.NotFoundException">No shop has that id.</exception>
    /// <exception cref="Exceptions.InvalidRequestException">The niche is not one of the shop's niches.</exception>
    Task<ReviewModel> CreateAsync(CreateReviewRequestModel review, CancellationToken cancellationToken);
}
