using FluentValidation;

namespace AzureShoppingCart.Features.Brands.UpdateBrand;

public sealed class UpdateBrandValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(60).WithMessage("El nombre no puede exceder los 60 caracteres");
    }
}
