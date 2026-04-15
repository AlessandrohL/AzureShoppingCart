using AzureShoppingCart.Common;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AzureShoppingCart.Features.Users.LoginUser;

public sealed class LoginUserHandler(
    UserManager<IdentityUser> userManager,
    TokenProvider tokenProvider,
    ClaimsIdentityProvider claimsIdentityProvider)
    : IRequestHandler<LoginUserCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        IdentityUser? user = await userManager.FindByEmailAsync(request.Email);

        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Result.Failure<TokenResponse>(UserErrors.InvalidCredentials);
        }

        ClaimsIdentity userClaims = await claimsIdentityProvider.CreateClaims(user);
        string accessToken = tokenProvider.CreateToken(userClaims);

        var response = new TokenResponse(accessToken, string.Empty);

        return Result.Success(response);
    }
}
