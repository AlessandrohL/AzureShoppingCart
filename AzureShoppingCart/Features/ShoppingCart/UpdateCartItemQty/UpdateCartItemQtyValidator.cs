using FluentValidation;

namespace AzureShoppingCart.Features.ShoppingCart.UpdateCartItemQty;

public sealed class UpdateCartItemQtyValidator : AbstractValidator<UpdateCartItemQtyCommand>
{
    public UpdateCartItemQtyValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("El Id del producto no es válido");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("La cantidad mínima requerida es de 1")
            .LessThanOrEqualTo(100)
            .WithMessage("La cantidad no puede ser mayor a 100");
    }
}
