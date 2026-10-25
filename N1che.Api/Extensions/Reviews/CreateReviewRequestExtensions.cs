using N1che.Contracts.Requests.Reviews;
using N1che.Domain.Models.Reviews;
using N1che.Domain.Models.Users;

namespace N1che.Api.Extensions.Reviews;

public static class CreateReviewRequestExtensions
{
    public static CreateReviewRequestModel ToDomainModel(this CreateReviewRequest request, Guid shopId, UserModel author) => new()
    {
        ShopId = shopId,
        Niche = request.Niche,
        Text = request.Text,
        UserId = author.Id,
        Username = author.Username,
    };
}
