using N1che.Domain.Exceptions;
using N1che.Domain.Interfaces.Persistence.Readers.Shops;
using N1che.Domain.Interfaces.Persistence.Writers.Reviews;
using N1che.Domain.Interfaces.Services.Reviews;
using N1che.Domain.Models.Reviews;

namespace N1che.Domain.Services.Reviews;

public sealed class ReviewCreationService : IReviewCreationService
{
    private readonly IShopsReader _shopsReader;
    private readonly IReviewsWriter _reviewsWriter;

    public ReviewCreationService(IShopsReader shopsReader, IReviewsWriter reviewsWriter)
    {
        _shopsReader = shopsReader;
        _reviewsWriter = reviewsWriter;
    }

    public async Task<ReviewModel> CreateAsync(CreateReviewRequestModel review, CancellationToken cancellationToken)
    {
        var shop = await _shopsReader.GetById(review.ShopId, cancellationToken)
                   ?? throw new NotFoundException(EntityTypes.Shop, review.ShopId);

        if (!shop.Niches.Contains(review.Niche))
        {
            throw new InvalidRequestException(EntityTypes.Review, review.ShopId, review.Niche);
        }

        return await _reviewsWriter.Create(review, cancellationToken);
    }
}
