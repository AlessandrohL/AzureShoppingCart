using AzureShoppingCart.Common;
using MediatR;

namespace AzureShoppingCart.Features.Brands.GetBrandById;

public record GetBrandByIdQuery(int Id) : IRequest<Result<GetBrandByIdResponse>>;