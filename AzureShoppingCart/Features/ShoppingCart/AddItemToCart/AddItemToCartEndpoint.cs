using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.ShoppingCart.AddItemToCart;

using AddItemToCartResult = Results<
    Ok<SuccessResponse<ShoppingCartCache>>,
    ProblemHttpResult>;

public sealed class AddItemToCartEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("cart", async Task<AddItemToCartResult> (
            [FromBody] AddItemToCartCommand command,
            ISender sender) =>
        {
            Result<ShoppingCartCache> result = await sender.Send(command);

            return result.IsSuccess
                ? ApiResults.Ok(result.Value)
                : ApiResults.Problem(result.Error);
        })
            .WithTags(EndpointTags.Cart)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Customer))
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .MapToApiVersion(1);
    }
}
