using System.Text.Json.Nodes;
using Microsoft.AspNetCore.OpenApi;
using N1che.Contracts.Pagination;
using N1che.Contracts.Requests.Shops;
using N1che.Contracts.Response.Shops;

namespace N1che.Api.OpenApi;

/// <summary>The payloads Scalar shows for the shops endpoints.</summary>
internal static class ShopExamples
{
    private const string ShopId = "0f5f3a1e-6f1a-4a2e-9c3d-2b6a1c4d5e6f";
    private const string GooglePlaceId = "ChIJ2eUgeAK6j4ARbn5u_wAGqWA";

    internal static OpenApiOptions AddShopExamples(this OpenApiOptions options) =>
        options.AddSchemaExamples(new Dictionary<Type, Func<JsonNode>>
        {
            [typeof(CreateShopRequest)] = AddShopRequest,
            [typeof(ShopResponse)] = Shop,
            [typeof(PagedResponse<ShopResponse>)] = ShopsPage,
            [typeof(ShopDetailResponse)] = ShopDetail,
            [typeof(ShopInteractionsResponse)] = ShopInteractions
        });

    private static JsonNode AddShopRequest() => new JsonObject
    {
        ["googlePlaceId"] = GooglePlaceId,
        ["niches"] = new JsonArray("goth", "vintage")
    };

    // A listed shop is its detail plus the live vote count the detail leaves out.
    private static JsonNode Shop()
    {
        var shop = ShopDetail();
        shop["voteCount"] = 128;

        return shop;
    }

    private static JsonNode ShopsPage() => new JsonObject
    {
        ["data"] = new JsonArray(Shop()),
        ["paginationDetails"] = new JsonObject
        {
            ["totalCount"] = 42,
            ["pageSize"] = 20,
            ["currentPage"] = 1
        }
    };

    private static JsonNode ShopDetail() => new JsonObject
    {
        ["id"] = ShopId,
        ["googlePlaceId"] = GooglePlaceId,
        ["name"] = "Void & Velvet",
        ["niches"] = new JsonArray("goth", "vintage"),
        ["address"] = "14 Tib Street, Manchester M4 1SH",
        ["latitude"] = 53.4842,
        ["longitude"] = -2.2364,
        ["placeStatus"] = "operational",
        ["openTime"] = "10:00:00",
        ["closeTime"] = "18:00:00",
        ["createdAt"] = "2026-09-02T14:31:07.215Z",
        ["addedByUserId"] = "a4f1c2d3-9b8e-4c7a-86d5-1e2f3a4b5c6d",
        ["addedByUsername"] = "velvetghoul",
        ["photoUrl"] = null
    };

    private static JsonNode ShopInteractions() => new JsonObject
    {
        ["voteCount"] = 128,
        ["voted"] = true,
        ["saved"] = false
    };
}
