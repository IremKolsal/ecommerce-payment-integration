using MediatR;

namespace ECommerce.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(
    int Amount,      
    string OrderId
) : IRequest<CreateOrderCommandResult>;

