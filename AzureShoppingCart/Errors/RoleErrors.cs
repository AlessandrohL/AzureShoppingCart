using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound = new(
        "Role.NotFound",
        "El rol especificado no existe.",
        ErrorType.NotFound);
}
