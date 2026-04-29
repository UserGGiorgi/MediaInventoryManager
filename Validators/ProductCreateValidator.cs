using FluentValidation;
using MediaInventoryManager.DTOs;

namespace MediaInventoryManager.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title must be under 200 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description is too long.");

        }
    }
}
