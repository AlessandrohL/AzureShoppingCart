using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Features.Brands.GetBrandById;

public sealed class GetBrandByIdEndpoint : IEndpoint
{
    public static readonly string Name = "GetBrandById";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("brands/{brandId}",
            async Task<Results<Ok<SuccessResponse<GetBrandByIdResponse>>, ProblemHttpResult>> (int brandId, ISender sender) =>
        {
            var result = await sender.Send(new GetBrandByIdQuery(brandId));

            return result.IsSuccess
                ? ApiResults.Ok(result.Value)
                : ApiResults.Problem(result.Error);
        })
            .WithName(Name)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithTags(EndpointTags.Brands)
            .MapToApiVersion(1);
    }
}
