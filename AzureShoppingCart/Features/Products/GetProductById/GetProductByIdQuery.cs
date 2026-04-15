using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Products.GetProductById;

public record GetProductByIdQuery(int ProductId) : IRequest<Result<GetProductByIdResponse>>;