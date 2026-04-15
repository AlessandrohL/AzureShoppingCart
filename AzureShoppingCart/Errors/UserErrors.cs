using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "El correo electrónico o la contraseña son incorrectos.",
        ErrorType.Unauthorized);
}
