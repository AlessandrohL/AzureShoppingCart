using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Products.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    int BrandId,
    decimal Price,
    int Stock,
    IFormFile Image) : IRequest<Result<int>>;