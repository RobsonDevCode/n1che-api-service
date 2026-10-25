using N1che.Contracts.Requests.Reviews;
using N1che.Domain.Models.Reviews;
using N1che.Domain.Models.Users;

namespace N1che.Api.Extensions.Reviews;

public static class UpdateReviewRequestExtensions
{
    public static UpdateReviewRequestModel ToDomainModel(this UpdateReviewRequest request, Guid shopId, Guid reviewId, UserModel author) => new()
    {
        Id = reviewId,
        ShopId = shopId,
        Text = request.Text,
        UserId = author.Id,
    };
}
