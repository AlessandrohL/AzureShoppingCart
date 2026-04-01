using AzureShoppingCart.Entities;
using System.Linq.Expressions;

namespace AzureShoppingCart.Features.Brands.GetBrands;

public static class GetBrandsProjections
{
    public static Expression<Func<Brand, GetBrandsResponse>> ToResponse =>
        brand => new GetBrandsResponse(brand.Id, brand.Name);
}
