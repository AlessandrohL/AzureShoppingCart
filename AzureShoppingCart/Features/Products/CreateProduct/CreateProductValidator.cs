using AzureShoppingCart.Extensions;
using FluentValidation;

namespace AzureShoppingCart.Features.Products.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(300).WithMessage("El nombre no debe tener más de 300 caracteres");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es requerida")
            .MaximumLength(500).WithMessage("La descripción no debe tener más de 500 caracteres");

        RuleFor(x => x.BrandId)
            .GreaterThan(0).WithMessage("El Id de la marca es invalida");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

        RuleFor(x => x.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("El stock no puede ser negativo");

        RuleFor(x => x.Image)
            .NotNull().WithMessage("La imagen es obligatoria")
            .Must(file => file!.IsImage()).WithMessage("El archivo debe ser una imagen válida");
    }
}
