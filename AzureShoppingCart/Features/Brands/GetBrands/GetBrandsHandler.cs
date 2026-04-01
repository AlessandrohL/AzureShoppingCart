using AzureShoppingCart.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Features.Brands.GetBrands;

public sealed class GetBrandsHandler(ApplicationDbContext context)
    : IRequestHandler<GetBrandsQuery, IEnumerable<GetBrandsResponse>>
{
    public async Task<IEnumerable<GetBrandsResponse>> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
    {
        return await context.Brands
            .AsNoTracking()
            .Select(GetBrandsProjections.ToResponse)
            .ToListAsync(cancellationToken);
    }
}
