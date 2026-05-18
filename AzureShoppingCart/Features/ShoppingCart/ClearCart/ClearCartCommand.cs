using MediatR;

namespace AzureShoppingCart.Features.ShoppingCart.ClearCart;

public record ClearCartCommand : IRequest<ShoppingCartCache>;
