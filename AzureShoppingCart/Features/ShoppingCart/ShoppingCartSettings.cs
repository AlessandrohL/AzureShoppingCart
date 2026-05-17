using Microsoft.Extensions.Caching.Distributed;

namespace AzureShoppingCart.Features.ShoppingCart;

public static class ShoppingCartSettings
{
    public static readonly DistributedCacheEntryOptions Options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
    };
}
