using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Extensions;

public static class UserManagerExtensions
{
    public static async Task<bool> IsEmailInUseAsync(
        this UserManager<IdentityUser> userManager,
        string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        string normalizedEmail = userManager.NormalizeEmail(email);

        return await userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail);
    }
}
