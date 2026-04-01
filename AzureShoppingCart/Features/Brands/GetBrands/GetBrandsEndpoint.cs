using AzureShoppingCart.Common;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Interfaces;

namespace AzureShoppingCart.Features.Brands.GetBrands;

public sealed class GetBrandsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("brands", async (CancellationToken cancellationToken) =>
        {
            ICollection<Brand> data = [new Brand { Id = 1, Name = "Huawei", Products = [] }];

            return Results.Ok(data);
        })
            .WithTags(EndpointTags.Brands)
            .MapToApiVersion(1);
    }
}