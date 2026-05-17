using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Extensions;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace AzureShoppingCart.Features.ShoppingCart.GetCart;

public sealed class GetCartHandler(
    ApplicationDbContext dbContext,
    IDistributedCache cache,
    IUserContext userContext)
    : IRequestHandler<GetCartQuery, ShoppingCartCache>
{
    public async Task<ShoppingCartCache> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.ShoppingCart(userContext.CustomerId);

        ShoppingCartCache? shoppingCart = await cache.GetOrCreateAsync(cacheKey, async ct =>
        {
            ShoppingCartCache? cart = await dbContext.Carts
                .Where(c => c.CustomerId == userContext.CustomerId)
                .Select(ShoppingCartProjections.ToCache)
                .FirstOrDefaultAsync(ct);

            return cart ?? ShoppingCartCache.Empty();
        },
        ShoppingCartSettings.Options,
        cancellationToken);

        return shoppingCart!;
    }
}
