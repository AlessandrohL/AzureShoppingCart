namespace AzureShoppingCart.Common;

public static class CacheKeys
{
    public static string ShoppingCart(Guid customerId) => $"shoppingcart:{customerId}";
}
