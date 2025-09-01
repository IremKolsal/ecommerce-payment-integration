using ECommerce.Application.Abstractions;
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
        // 1) Siparişi repo’dan al
        var order = await _repository.GetByExternalIdAsync(command.OrderId, cancellationToken)
                     ?? throw new KeyNotFoundException("Order not found");

        // 2) Durum kontrolü
        if (!string.Equals(order.Status, "blocked", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Order state is not blocked");

        // 3) Upstream tamamla (DTO yok, primitive parametre)
        var res = await _balance.CompleteAsync(order.ExternalOrderId, cancellationToken);

        // 4) Domain güncelle
        order.Status = res.Status;
        order.CompletedAt = res.CompletedAt;

        await _repository.SaveChangesAsync(cancellationToken);

        return new CompleteOrderCommandResult(order.ExternalOrderId, order.Status);
    }
}
