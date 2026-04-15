namespace AzureShoppingCart.Features.Products.GetProductById;

public record GetProductByIdResponse(
    int Id,
    string Name,
    string Description,
    string Slug,
    string BrandName,
    decimal Price,
    int Stock,
    string ImageUrl,
    DateTimeOffset CreatedAt);