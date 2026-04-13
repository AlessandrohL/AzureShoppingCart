using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using AzureShoppingCart.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Features.Products.CreateProduct;

public sealed class CreateProductHandler(
    ApplicationDbContext context,
    IImageStorageService imageStorageService)
    : IRequestHandler<CreateProductCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        string slug = request.Name.GenerateSlug();

        if (await context.Products.AnyAsync(p => p.Slug == slug, cancellationToken))
        {
            return Result.Failure<int>(ProductErrors.DuplicateSlug);
        }

        if (!await context.Brands.AnyAsync(b => b.Id == request.BrandId, cancellationToken))
        {
            return Result.Failure<int>(BrandErrors.NotFound);
        }

        using var stream = request.Image.OpenReadStream();

        string imageUrl = await imageStorageService.UploadAsync(
            stream, 
            request.Image.FileName, 
            request.Image.ContentType);

        var newProduct = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Slug = slug,
            BrandId = request.BrandId,
            Price = request.Price,
            Stock = request.Stock,
            ImageUrl = imageUrl
        };

        context.Products.Add(newProduct);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success(newProduct.Id);
    }
}
