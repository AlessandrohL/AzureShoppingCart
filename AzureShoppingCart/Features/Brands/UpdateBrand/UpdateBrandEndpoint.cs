using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Brands.UpdateBrand;

using UpdateBrandResult = Results<Ok<SuccessResponse>, ProblemHttpResult>;

public sealed class UpdateBrandEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("brands/{brandId}", async Task<UpdateBrandResult> (
            int brandId,
            [FromBody] UpdateBrandRequest request,
            ISender sender) =>
        {
            Result updateResult = await sender.Send(new UpdateBrandCommand(
                brandId,
                request));

            return updateResult.IsSuccess
                ? ApiResults.Ok()
                : ApiResults.Problem(updateResult.Error);
        })
            .WithTags(EndpointTags.Brands)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Admin))
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .MapToApiVersion(1);
    }
}
