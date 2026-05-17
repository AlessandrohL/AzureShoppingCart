using FluentValidation;

namespace AzureShoppingCart.Features.ShoppingCart.AddItemToCart;

public sealed class AddItemToCartValidator : AbstractValidator<AddItemToCartCommand>
{
    public AddItemToCartValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("El Id del producto no es válido.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad mínima requerida es de 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("La cantidad no puede ser mayor a 100.");
    }
}
