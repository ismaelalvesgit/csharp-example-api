using Example.Application.Dto;
using FluentValidation;

namespace Example.Application.Validations
{
    public class CategoryValidation : AbstractValidator<CategoryDto>
    {
        public CategoryValidation() { 
            RuleFor(x => x.Name).NotNull().WithMessage("Name is required");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is not empty");

            RuleFor(x => x.ImageUrl).NotNull().WithMessage("ImageUrl is required");
            RuleFor(x => x.ImageUrl).NotEmpty().WithMessage("ImageUrl is not empty");
        }
    }
}
