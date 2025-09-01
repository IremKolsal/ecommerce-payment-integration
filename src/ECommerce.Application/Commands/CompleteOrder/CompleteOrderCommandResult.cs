namespace ECommerce.Application.Commands.CompleteOrder;

public sealed record CompleteOrderCommandResult(
    string OrderId,
    string Status
);

