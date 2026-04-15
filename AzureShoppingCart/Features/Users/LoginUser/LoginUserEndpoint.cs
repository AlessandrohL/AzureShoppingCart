using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AzureShoppingCart.Features.Users.LoginUser;

using LoginUserResult = Results<
    Ok<SuccessResponse<TokenResponse>>,
    ProblemHttpResult>;

public sealed class LoginUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("users/login", async Task<LoginUserResult> (
            [FromBody] LoginUserCommand command,
            ISender sender) =>
        {
            var loginResult = await sender.Send(command);

            return loginResult.IsSuccess
                ? ApiResults.Ok(loginResult.Value)
                : ApiResults.Problem(loginResult.Error);
        })
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .WithTags(EndpointTags.Users)
            .MapToApiVersion(1);
    }
}
