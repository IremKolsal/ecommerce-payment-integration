using ECommerce.Application.Contracts;
using FluentValidation;

namespace ECommerce.Application.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("orderId is required")
            .MaximumLength(64).WithMessage("orderId must be at most 64 chars");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("amount must be greater than 0")
            .LessThanOrEqualTo(1_000_000).WithMessage("amount is too large");
    }
}