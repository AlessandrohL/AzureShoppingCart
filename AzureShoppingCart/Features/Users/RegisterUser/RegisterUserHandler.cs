using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AzureShoppingCart.Features.Users.RegisterUser;

public sealed class RegisterUserHandler(
    ApplicationDbContext dbContext,
    UserManager<IdentityUser> userManager,
    RoleManager<IdentityRole> roleManager)
    : IRequestHandler<RegisterUserCommand, Result>
{
    public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.IsEmailInUseAsync(request.Email))
        {
            return Result.Failure(UserErrors.EmailAlreadyInUse);
        }

        IdentityRole? role = await roleManager.FindByIdAsync(request.RoleId);

        if (role is null)
        {
            return Result.Failure(RoleErrors.NotFound);
        }

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var newUser = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email,
            NormalizedEmail = userManager.NormalizeEmail(request.Email),
            NormalizedUserName = userManager.NormalizeName(request.Email)
        };

        IdentityResult addUserResult = await userManager.CreateAsync(newUser, request.Password);

        if (!addUserResult.Succeeded)
        {
            return Result.Failure(new ValidationError(addUserResult.ToErrors()));
        }

        IdentityResult addToRoleResult = await userManager.AddToRoleAsync(newUser, role.Name!);

        if (!addToRoleResult.Succeeded)
        {
            return Result.Failure(new ValidationError(addToRoleResult.ToErrors()));
        }

        await transaction.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
