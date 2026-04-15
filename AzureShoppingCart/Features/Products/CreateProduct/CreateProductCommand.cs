using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Products.CreateProduct;

public sealed class CreateProductCommand : IRequest<Result<int>>
{
    public string? Name { get; init; }
    public string? Description { get; init; }
    public int BrandId { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public IFormFile? Image { get; init; }
}