using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class ProductErrors
{
    public static readonly Error NotFound = new(
        "Products.NotFound", 
        "The requested product was not found", 
        ErrorType.NotFound);

    public static readonly Error DuplicateSlug = new(
        "Products.DuplicateSlug",
        "A product with the same slug already exists",
        ErrorType.Conflict);
}
