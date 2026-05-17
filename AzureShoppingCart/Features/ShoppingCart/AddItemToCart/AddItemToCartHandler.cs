using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AzureShoppingCart.Features.ShoppingCart.AddItemToCart;

public sealed class AddItemToCartHandler(
    ApplicationDbContext dbContext,
    IDistributedCache cache,
    IUserContext userContext,
    TimeProvider timeProvider)
    : IRequestHandler<AddItemToCartCommand, Result<ShoppingCartCache>>
{
    public async Task<Result<ShoppingCartCache>> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
    {
        Product? product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ShoppingCartCache>(ProductErrors.NotFound);
        }

        if (!product.HasStock)
        {
            return Result.Failure<ShoppingCartCache>(CartErrors.ProductSoldOut(product.Name));
        }

        Cart? cart = await dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == userContext.CustomerId, cancellationToken);

        if (cart is null)
        {
            cart = Cart.Create(userContext.CustomerId, timeProvider);
            dbContext.Carts.Add(cart);
        }

        CartItem? existingItem = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);

        int alreadyInCart = existingItem?.Quantity ?? 0;
        int totalRequested = alreadyInCart + request.Quantity;

        if (totalRequested > product.Stock)
        {
            return Result.Failure<ShoppingCartCache>(CartErrors.InsufficientStock(product.Name));
        }

        if (existingItem is null)
        {
            cart.AddItem(product.Id, request.Quantity);
        }
        else
        {
            existingItem.AddQuantity(request.Quantity);
        }

        cart.UpdatedAt = timeProvider.GetUtcNow();

        await dbContext.SaveChangesAsync(cancellationToken);
        
        string cacheKey = CacheKeys.ShoppingCart(userContext.CustomerId);

        ShoppingCartCache cachedCart = await dbContext.Carts
            .Where(c => c.Id == cart.Id)
            .Select(ShoppingCartProjections.ToCache)
            .FirstAsync(cancellationToken);

        await cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(cachedCart),
            ShoppingCartSettings.Options,
            cancellationToken);

        return Result.Success(cachedCart);
    }
}
