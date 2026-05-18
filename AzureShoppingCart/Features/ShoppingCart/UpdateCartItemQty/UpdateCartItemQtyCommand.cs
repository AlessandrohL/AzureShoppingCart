using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.ShoppingCart.UpdateCartItemQty;

public sealed class UpdateCartItemQtyCommand : IRequest<Result<ShoppingCartCache>>
{
    public required int ProductId { get; set; }
    public required int Quantity { get; set; }
}
