using ECommerce.Application.Contracts;
using FluentValidation;

namespace ECommerce.Application.Validation;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
{
    //public CreateOrderRequestValidator()
    //{
    //    RuleFor(x => x.Items)
    //        .NotEmpty();

    //    RuleForEach(x => x.Items)
    //        .SetValidator(new CreateOrderItemDtoValidator());
    //}
}

public class CreateOrderItemDtoValidator : AbstractValidator<CreateOrderItemDto>
{
    public CreateOrderItemDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}
