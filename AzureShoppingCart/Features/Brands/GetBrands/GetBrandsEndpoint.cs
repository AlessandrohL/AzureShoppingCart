using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AzureShoppingCart.Features.Brands.GetBrands;

using GetBrandsResult = Results<
    Ok<SuccessResponse<IEnumerable<GetBrandsResponse>>>,
    UnauthorizedHttpResult,
    ForbidHttpResult>;

public sealed class GetBrandsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("brands", async Task<GetBrandsResult> (ISender sender) =>
        {
            var brands = await sender.Send(new GetBrandsQuery());

            return ApiResults.Ok(brands);
        })
            .WithTags(EndpointTags.Brands)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Admin))
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(1);
    }
}