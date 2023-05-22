using Example.Application.Dto;
using FluentValidation;

namespace Example.Application.Validations
{
    public class ProductValidation : AbstractValidator<ProductDto>
    {
        public ProductValidation() { 
            RuleFor(x => x.CategoryId).NotNull().WithMessage("CategoryId is required");
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("CategoryId is not empty");
            RuleFor(x => x.CategoryId).GreaterThanOrEqualTo(0).WithMessage("CategoryId GreaterThanOrEqualTo 0");

            RuleFor(x => x.Name).NotNull().WithMessage("Name is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is not empty");

            RuleFor(x => x.ImageUrl).NotNull().WithMessage("ImageUrl is required");
            RuleFor(x => x.ImageUrl).NotEmpty().WithMessage("ImageUrl is not empty");

            RuleFor(x => x.Quantity).NotNull().WithMessage("Quantity is required");
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity GreaterThanOrEqualTo 0");

            RuleFor(x => x.Price).NotNull().WithMessage("Price is required");
            RuleFor(x => x.Price).NotEqual(0).When(x => x.Quantity > 0).WithMessage("Please specify a price");
        }
    }
}
