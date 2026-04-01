using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using AzureShoppingCart.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Features.Brands.GetBrandById;

public sealed class GetBrandByIdHandler(ApplicationDbContext context)
    : IRequestHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
{
    public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery request, CancellationToken cancellationToken)
    {
        Brand? brand = await context.Brands
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (brand is null)
        {
            return Result.Failure<GetBrandByIdResponse>(BrandErrors.NotFound);
        }

        return Result.Success(new GetBrandByIdResponse(brand.Id, brand.Name));
    }
}
