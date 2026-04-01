using MediatR;

namespace AzureShoppingCart.Features.Brands.GetBrands;

public record GetBrandsQuery : IRequest<IEnumerable<GetBrandsResponse>>;