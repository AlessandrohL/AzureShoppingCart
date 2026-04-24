using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Users.RegisterUser;

public sealed class RegisterUserCommand : IRequest<Result>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string PhoneNumber { get; init; }
    public required string RoleId { get; init; }
}
