using AzureShoppingCart.Common;
using AzureShoppingCart.Interfaces;
using MediatR;

namespace AzureShoppingCart.Features.Brands.GetBrands;

public sealed class GetBrandsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("brands", async (ISender sender) =>
        {
            var brands = await sender.Send(new GetBrandsQuery());

            return ApiResults.Ok(brands);
        })
            .WithTags(EndpointTags.Brands)
            .MapToApiVersion(1);
    }
}