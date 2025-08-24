using ECommerce.Application.Contracts;
using ECommerce.Application.Services;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Balance;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IBalanceClient _balance;

    public OrderService(AppDbContext db, IBalanceClient balance)
    {
        _db = db;
        _balance = balance;
    }

    public async Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await _balance.GetProductsAsync(cancellationToken);
        return products;
    }

    public async Task<CreateOrderResponseDto> CreateAsync(CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        var preReq = new PreorderRequestDto(request.Amount, request.OrderId);
        var preRes = await _balance.PreorderAsync(preReq, cancellationToken);

        var status = preRes.Data.PreOrder.Status;
        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = status,
            TotalAmount = preRes.Data.PreOrder.Amount,
            ExternalOrderId = preRes.Data.PreOrder.OrderId,
            ReservedAt = DateTime.UtcNow
        };

        await _db.Orders.AddAsync(order, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponseDto(order.Id, order.ExternalOrderId, order.Status, order.TotalAmount);
    }

    public async Task<CompleteOrderResponseDto> CompleteAsync(string orderId, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.ExternalOrderId == orderId, cancellationToken) ?? throw new KeyNotFoundException("Order not found");

        if (!string.Equals(order.Status, "blocked", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Order state is not blocked");

        var res = await _balance.CompleteAsync(new CompleteRequestDto(order.ExternalOrderId), cancellationToken);

        order.Status = res.Data.Order.Status;
        order.CompletedAt = res.Data.Order.CompletedAt;

        await _db.SaveChangesAsync(cancellationToken);

        return new CompleteOrderResponseDto(order.ExternalOrderId, order.Status);
    }
}
