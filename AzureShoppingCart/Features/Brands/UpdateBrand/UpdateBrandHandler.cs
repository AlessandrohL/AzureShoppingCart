using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using MediatR;

namespace AzureShoppingCart.Features.Brands.UpdateBrand;

public sealed class UpdateBrandHandler(ApplicationDbContext context)
    : IRequestHandler<UpdateBrandCommand, Result>
{
    public async Task<Result> Handle(UpdateBrandCommand request, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands.FindAsync([request.BrandId], cancellationToken);

        if (brand is null)
        {
            return Result.Failure(BrandErrors.NotFound);
        }

        brand.Name = request.Update.Name;

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
