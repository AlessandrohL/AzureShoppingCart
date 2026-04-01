using AzureShoppingCart.Data;
using AzureShoppingCart.Entities;
using MediatR;

namespace AzureShoppingCart.Features.Brands.CreateBrand;

public sealed class CreateBrandHandler(ApplicationDbContext context) 
    : IRequestHandler<CreateBrandCommand, int>
{
    public async Task<int> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var newBrand = new Brand { Name = request.Name };

        context.Brands.Add(newBrand);
        await context.SaveChangesAsync(cancellationToken);

        return newBrand.Id;
    }
}
