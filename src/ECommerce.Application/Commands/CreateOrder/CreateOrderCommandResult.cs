namespace ECommerce.Application.Commands.CreateOrder;

public record CreateOrderCommandResult(
    Guid Id,
    string OrderId,
    string Status,
    decimal TotalAmount
);
