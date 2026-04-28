using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Customers.RegisterCustomer;

public sealed class RegisterCustomerCommand : IRequest<Result>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}
