using AzureShoppingCart.Entities;
using System.Linq.Expressions;

namespace AzureShoppingCart.Features.ShoppingCart;

public static class ShoppingCartProjections
{
    public static readonly Expression<Func<Cart, ShoppingCartCache>> ToCache =
        cart => new ShoppingCartCache
        {
            Id = cart.Id.ToString(),
            Items = cart.Items
                .Select(ci => new ShoppingCartItemCache
                {
                    Id = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product!.Name,
                    UnitPrice = ci.Product.Price,
                    Quantity = ci.Quantity,
                })
                .ToList()
        };
}
