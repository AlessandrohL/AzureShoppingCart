using AzureShoppingCart.Common;
using AzureShoppingCart.Features.Brands.GetBrandById;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Brands.CreateBrand;

public sealed class CreateBrandEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("brands", async (
            [FromBody] CreateBrandCommand command, 
            ILinkService linkService,
            ISender sender) =>
        {
            int createdBrandId = await sender.Send(command);

            Link link = linkService.Generate(
                GetBrandByIdEndpoint.Name,
                new { brandId = createdBrandId },
                rel: "self",
                method: HttpMethods.Get);

            return ApiResults.Created(link.Href);
        })
            .WithTags(EndpointTags.Brands)
            .MapToApiVersion(1);
    }
}
