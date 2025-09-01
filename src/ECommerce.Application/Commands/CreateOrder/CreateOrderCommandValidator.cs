using FluentValidation;

namespace ECommerce.Application.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("orderId is required")
            .MaximumLength(64).WithMessage("orderId must be at most 64 chars");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("amount must be greater than 0")
            .LessThanOrEqualTo(1_000_000).WithMessage("amount is too large");
    }
}