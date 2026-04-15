using AzureShoppingCart.Common;
using AzureShoppingCart.Features.Products.GetProductById;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Products.CreateProduct;

using CreateProductResult = Results<Created<SuccessResponse>, ProblemHttpResult>;

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("products", async Task<CreateProductResult> (
            [FromForm] CreateProductCommand command,
            ILinkService linkService,
            ISender sender) =>
        {
            Result<int> createdResult = await sender.Send(command);

            if (createdResult.IsFailure)
            {
                return ApiResults.Problem(createdResult.Error);
            }

            Link link = linkService.Generate(
                GetProductByIdEndpoint.Name,
                new { productId = createdResult.Value },
                rel: "self",
                method: HttpMethods.Get);

            return ApiResults.Created(link.Href);
        })
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(EndpointTags.Products)
            .MapToApiVersion(1)
            .DisableAntiforgery();
    }
}
