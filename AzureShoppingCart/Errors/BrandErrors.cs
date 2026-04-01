using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class BrandErrors
{
    public static readonly Error NotFound = new(
        "Brands.NotFound", "The requested brand was not found", ErrorType.NotFound);
}
