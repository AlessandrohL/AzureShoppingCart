using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Features.ShoppingCart.ClearCart;

using ClearCartResult = Results<
    Ok<SuccessResponse<ShoppingCartCache>>,
    UnauthorizedHttpResult>;

public sealed class ClearCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("cart/clear", async Task<ClearCartResult> (ISender sender) =>
        {
            ShoppingCartCache cart = await sender.Send(new ClearCartCommand());

            return ApiResults.Ok(cart);
        })
            .WithTags(EndpointTags.Cart)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Customer))
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(1);
    }
}
