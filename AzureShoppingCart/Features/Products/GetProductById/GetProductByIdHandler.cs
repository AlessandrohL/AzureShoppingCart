using AzureShoppingCart.Common;
using AzureShoppingCart.Data;
using AzureShoppingCart.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AzureShoppingCart.Features.Products.GetProductById;

public sealed class GetProductByIdHandler(
    ApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .AsNoTracking()
            .Where(p => p.Id == request.ProductId)
            .Select(GetProductByIdProjection.ToResponse)
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
        {
            return Result.Failure<GetProductByIdResponse>(ProductErrors.NotFound);
        }

        return Result.Success(product);
    }
}
