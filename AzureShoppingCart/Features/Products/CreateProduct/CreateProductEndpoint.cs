using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Products.CreateProduct;

using CreateProductResult = Results<Ok<SuccessResponse>, ProblemHttpResult>;

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

            return createdResult.IsSuccess
                ? ApiResults.Ok()
                : ApiResults.Problem(createdResult.Error);
        })
            .WithTags(EndpointTags.Products)
            .MapToApiVersion(1)
            .DisableAntiforgery();
    }
}
