using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.ShoppingCart.AddItemToCart;

public sealed class AddItemToCartCommand : IRequest<Result<ShoppingCartCache>>
{
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
}