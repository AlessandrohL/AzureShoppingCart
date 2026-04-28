using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new(
        "User.InvalidCredentials",
        "El correo electrónico o la contraseña son incorrectos.",
        ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyInUse = new(
        "User.EmailAlreadyInUse",
        "El correo electrónico ya se encuentra registrado.",
        ErrorType.Conflict);
}
