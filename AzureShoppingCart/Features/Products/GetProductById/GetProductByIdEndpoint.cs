using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Features.Products.GetProductById;

using GetProductByIdResult = Results<
    Ok<SuccessResponse<GetProductByIdResponse>>,
    ProblemHttpResult>;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public static readonly string Name = "GetProductById";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("products/{productId}",
            async Task<GetProductByIdResult> (int productId, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(productId));

            return result.IsSuccess
                ? ApiResults.Ok(result.Value)
                : ApiResults.Problem(result.Error);
        })
            .WithName(Name)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(EndpointTags.Products)
            .MapToApiVersion(1);
    }
}
