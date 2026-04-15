using AzureShoppingCart.Entities;
using System.Linq.Expressions;

namespace AzureShoppingCart.Features.Products.GetProductById;

public static class GetProductByIdProjection
{
    public static Expression<Func<Product, GetProductByIdResponse>> ToResponse =>
        product => new GetProductByIdResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Slug,
            product.Brand!.Name,
            product.Price,
            product.Stock,
            product.ImageUrl,
            product.CreatedAt);
}
