using MediatR;

namespace ECommerce.Application.Commands.CompleteOrder;

public sealed record CompleteOrderCommand(
    string OrderId
) : IRequest<CompleteOrderCommandResult>;
