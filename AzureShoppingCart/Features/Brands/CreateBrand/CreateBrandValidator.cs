using FluentValidation;

namespace AzureShoppingCart.Features.Brands.CreateBrand;

public sealed class CreateBrandValidator : AbstractValidator<CreateBrandCommand>
{
    public CreateBrandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido")
            .MaximumLength(60).WithMessage("El nombre no puede exceder los 60 caracteres");
    }
}
