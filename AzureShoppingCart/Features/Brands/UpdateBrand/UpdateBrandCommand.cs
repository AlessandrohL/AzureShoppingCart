using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Brands.UpdateBrand;

public record UpdateBrandCommand(int BrandId, UpdateBrandRequest Update) : IRequest<Result>;
