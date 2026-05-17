using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Features.ShoppingCart.GetCart;

using GetCartResult = Results<
    Ok<SuccessResponse<ShoppingCartCache>>,
    UnauthorizedHttpResult>;

public sealed class GetCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("cart", async Task<GetCartResult> (ISender sender) =>
        {
            ShoppingCartCache cart = await sender.Send(new GetCartQuery());

            return ApiResults.Ok(cart);
        })
            .WithTags(EndpointTags.Cart)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Customer))
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1);
    }
}
