using N1che.Contracts.Response.Routes;
using N1che.Domain.Models.Routes;

namespace N1che.Api.Extensions.Routes;

public static class RouteInteractionsResponseExtensions
{
    public static RouteInteractionsResponse ToResponse(this RouteInteractionsModel interactions) => new()
    {
        VoteCount = interactions.VoteCount,
        Voted = interactions.Voted,
    };
}
