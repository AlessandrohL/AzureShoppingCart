using AzureShoppingCart.Common;

namespace AzureShoppingCart.Errors;

public static class CartErrors
{
    public static Error ProductSoldOut(string productName) => new(
        "Cart.SoldOut",
        $"El producto '#{productName}' se encuentra agotado",
        ErrorType.UnprocessableContent);

    public static Error InsufficientStock(string productName) => new(
        "Cart.InsufficientStock",
        $"No hay suficiente stock para el producto '{productName}'",
        ErrorType.UnprocessableContent);

    public static Error NotFound => new(
        "Cart.NotFound",
        "No se encontró un carrito asociado",
        ErrorType.NotFound);
}
