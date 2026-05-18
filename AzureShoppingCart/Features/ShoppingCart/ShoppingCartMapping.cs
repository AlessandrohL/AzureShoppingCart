using AzureShoppingCart.Entities;

namespace AzureShoppingCart.Features.ShoppingCart;

public static class ShoppingCartMapping
{
    extension(Cart cart)
    {
        public ShoppingCartCache ToShoppingCartCache()
        {
            return new ShoppingCartCache
            {
                Id = cart.Id.ToString(),
                Items = cart.Items
                    .Select(ci => new ShoppingCartItemCache
                    {
                        Id = ci.Id,
                        ProductId = ci.ProductId,
                        ProductName = ci.Product!.Name,
                        Quantity = ci.Quantity,
                        UnitPrice = ci.Product.Price
                    })
                    .ToList()
            };
        }
    }
}
