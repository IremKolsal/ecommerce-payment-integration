using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using MediatR;

namespace ECommerce.Application.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResult>
{
    private readonly IBalanceClient _balance;
    private readonly IOrderRepository _repository;

    public CreateOrderCommandHandler(IBalanceClient balance, IOrderRepository repository)
    {
        _balance = balance;
        _repository = repository;
    }

    public async Task<CreateOrderCommandResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var preRes = await _balance.PreorderAsync(command.Amount, command.OrderId, cancellationToken);

        var status = preRes.Status;
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = status,
            TotalAmount = preRes.Amount,
            ExternalOrderId = preRes.ExternalOrderId,
            ReservedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(order, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateOrderCommandResult(order.Id, order.ExternalOrderId, order.Status, order.TotalAmount);
    }
}
