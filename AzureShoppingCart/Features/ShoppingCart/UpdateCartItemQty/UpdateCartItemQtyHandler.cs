using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Identity;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AzureShoppingCart.Features.ShoppingCart.UpdateCartItemQty;

public sealed class UpdateCartItemQtyHandler(
    ApplicationDbContext dbContext,
    IDistributedCache cache,
    IUserContext userContext,
    TimeProvider timeProvider)
    : IRequestHandler<UpdateCartItemQtyCommand, Result<ShoppingCartCache>>
{
    public async Task<Result<ShoppingCartCache>> Handle(UpdateCartItemQtyCommand request, CancellationToken cancellationToken)
    {
        Product? requestedProduct = await dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (requestedProduct is null)
        {
            return Result.Failure<ShoppingCartCache>(ProductErrors.NotFound);
        }

        if (request.Quantity > requestedProduct.Stock)
        {
            return Result.Failure<ShoppingCartCache>(CartErrors.InsufficientStock(requestedProduct.Name));
        }

        Cart? cart = await dbContext.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.CustomerId == userContext.CustomerId, cancellationToken);

        if (cart is null)
        {
            return Result.Failure<ShoppingCartCache>(CartErrors.NotFound);
        }

        CartItem? item = cart.Items.FirstOrDefault(ci => ci.ProductId == request.ProductId);

        if (item is null)
        {
            cart.AddItem(requestedProduct.Id, request.Quantity);
        }
        else
        {
            item.SetQuantity(request.Quantity);
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
