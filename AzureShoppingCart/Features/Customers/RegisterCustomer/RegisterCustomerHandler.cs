using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace AzureShoppingCart.Features.Customers.RegisterCustomer;

public sealed class RegisterCustomerHandler(
    ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager)
    : IRequestHandler<RegisterCustomerCommand, Result>
{
    public async Task<Result> Handle(RegisterCustomerCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.IsEmailInUseAsync(request.Email))
        {
            return Result.Failure(UserErrors.EmailAlreadyInUse);
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var newUser = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        IdentityResult addUserResult = await userManager.CreateAsync(newUser, request.Password);

        if (!addUserResult.Succeeded)
        {
            return Result.Failure(new ValidationError(addUserResult.ToErrors()));
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(newUser, AuthRoles.Customer);

        if (!addToRoleResult.Succeeded)
        {
            return Result.Failure(new ValidationError(addToRoleResult.ToErrors()));
        }

        var customer = new Customer
        {
            IdentityId = newUser.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
