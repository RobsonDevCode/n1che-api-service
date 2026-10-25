using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Pagination;
using N1che.Contracts.Requests.Reviews;
using N1che.Contracts.Response.Reviews;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the shop reviews endpoints.</summary>
internal static class ReviewExamples
{
    private const string ReviewText = "Racks of deadstock velvet and the best silver hardware in the Northern Quarter.";

    internal static OpenApiOptions AddReviewExamples(this OpenApiOptions options) =>
        options.AddSchemaExamples(new Dictionary<Type, Func<JsonNode>>
        {
            [typeof(CreateReviewRequest)] = AddReviewRequest,
            [typeof(UpdateReviewRequest)] = UpdateReviewRequest,
            [typeof(ReviewResponse)] = Review,
            [typeof(PagedResponse<ReviewResponse>)] = ReviewsPage
        });

    private static JsonNode AddReviewRequest() => new JsonObject
    {
        ["text"] = ReviewText,
        ["niche"] = "goth"
    };

    private static JsonNode UpdateReviewRequest() => new JsonObject
    {
        ["text"] = ReviewText
    };

    private static JsonNode Review() => new JsonObject
    {
        ["id"] = "3c4d5e6f-7a8b-4c9d-8e0f-1a2b3c4d5e6f",
        ["shopId"] = "0f5f3a1e-6f1a-4a2e-9c3d-2b6a1c4d5e6f",
        ["niche"] = "goth",
        ["userId"] = "b5e2d3c4-0a9f-4d8b-97e6-2f3a4b5c6d7e",
        ["username"] = "cryptkeeper",
        ["text"] = ReviewText,
        ["createdAt"] = "2026-10-03T17:42:51.904Z",
        ["updatedAt"] = "2026-10-03T17:42:51.904Z"
    };

    private static JsonNode ReviewsPage() => new JsonObject
    {
        ["data"] = new JsonArray(Review()),
        ["paginationDetails"] = new JsonObject
        {
            ["totalCount"] = 7,
            ["pageSize"] = 20,
            ["currentPage"] = 1
        }
    };
}
