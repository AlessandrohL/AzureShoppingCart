using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Users.RegisterUser;

// refactor to Created type result

using RegisterUserResult = Results<Ok<SuccessResponse>, ProblemHttpResult>;

public sealed class RegisterUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users", async Task<RegisterUserResult> (
            [FromBody] RegisterUserCommand command,
            ILinkService linkService,
            ISender sender) =>
        {
            Result registerResult = await sender.Send(command);

            return registerResult.IsSuccess
                ? ApiResults.Ok()
                : ApiResults.Problem(registerResult.Error);
        })
            .WithTags(EndpointTags.Users)
            .RequireAuthorization(policy => policy.RequireRole(AuthRoles.Admin))
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .MapToApiVersion(1);
    }
}
