using AzureShoppingCart.Common;
using AzureShoppingCart.Identity;
using MediatR;

namespace AzureShoppingCart.Features.Users.LoginUser;

public sealed class LoginUserCommand : IRequest<Result<TokenResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
