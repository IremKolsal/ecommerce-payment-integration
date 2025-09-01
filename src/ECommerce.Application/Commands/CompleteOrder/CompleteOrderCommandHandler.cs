using ECommerce.Application.Abstractions;
using ECommerce.Application.Common.Errors;
using MediatR;

namespace ECommerce.Application.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand, CompleteOrderCommandResult>
{
    private readonly IBalanceClient _balance;
    private readonly IOrderRepository _repository;
    public CompleteOrderCommandHandler(IBalanceClient balance, IOrderRepository repository)
    {
        _balance = balance;
        _repository = repository;
    }
    public async Task<CompleteOrderCommandResult> Handle(CompleteOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByExternalIdAsync(command.OrderId, cancellationToken)
                    ?? throw AppErrors.Order.NotFound(command.OrderId);

        if (!string.Equals(order.Status, "blocked", StringComparison.OrdinalIgnoreCase))
            throw AppErrors.Order.MustBeBlocked(order.ExternalOrderId, order.Status);

        var info = await _balance.CompleteAsync(order.ExternalOrderId, cancellationToken);

        order.Status = info.Status;
        order.CompletedAt = info.CompletedAt;

        await _repository.SaveChangesAsync(cancellationToken);

        return new CompleteOrderCommandResult(order.ExternalOrderId, order.Status);
    }
}
