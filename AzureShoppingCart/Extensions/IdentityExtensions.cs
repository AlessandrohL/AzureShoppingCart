using AzureShoppingCart.Common;
using Microsoft.AspNetCore.Identity;

namespace AzureShoppingCart.Extensions;

public static class IdentityExtensions
{
    public static Error[] ToErrors(this IdentityResult result)
    {
        return result.Errors
            .Select(e => new Error(
                e.Code,
                e.Description,
                ErrorType.Validation))
            .ToArray();
    }
}
