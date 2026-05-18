using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AzureShoppingCart.Features.ShoppingCart.ClearCart;

public sealed class ClearCartHandler(
    IDistributedCache cache,
    ApplicationDbContext dbContext,
    IUserContext userContext,
    TimeProvider timeProvider) 
    : IRequestHandler<ClearCartCommand, ShoppingCartCache>
{
    public async Task<ShoppingCartCache> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        string cacheKey = CacheKeys.ShoppingCart(userContext.CustomerId);
        ShoppingCartCache? cachedCart = await GetCartCacheAsync(cacheKey, cancellationToken);

        if (cachedCart is not null && cachedCart.IsEmpty)
        {
            return cachedCart;
        }

        Cart? cart = await dbContext.Carts
            .FirstOrDefaultAsync(c => c.CustomerId == userContext.CustomerId, cancellationToken);

        if (cart is null)
        {
            return ShoppingCartCache.Empty();
        }

        await dbContext.CartItems
            .Where(ci => ci.CartId == cart.Id)
            .ExecuteDeleteAsync(cancellationToken);

        cart.UpdatedAt = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync(cancellationToken);

        cachedCart = new ShoppingCartCache { Id = cart.Id.ToString() };

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(cachedCart),
            ShoppingCartSettings.Options,
            cancellationToken);

        return cachedCart;
    }

    public async Task<ShoppingCartCache?> GetCartCacheAsync(string cacheKey, CancellationToken ct = default)
    {
        string? cartJson = await cache.GetStringAsync(cacheKey, ct);

        return string.IsNullOrEmpty(cartJson)
            ? null
            : JsonSerializer.Deserialize<ShoppingCartCache>(cartJson);
    }
}
