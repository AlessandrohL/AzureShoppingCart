using MediatR;

namespace AzureShoppingCart.Features.ShoppingCart.GetCart;

public record GetCartQuery : IRequest<ShoppingCartCache>;
