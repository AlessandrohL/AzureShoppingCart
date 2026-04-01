using MediatR;

namespace AzureShoppingCart.Features.Brands.CreateBrand;

public record CreateBrandCommand(string Name) : IRequest<int>;