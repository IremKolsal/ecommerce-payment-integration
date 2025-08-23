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
        var pre = await _balance.PreorderAsync(new PreorderRequestDto(request.Amount, request.OrderId), cancellationToken);

        var model = pre.Data;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Status = "Reserved",
            TotalAmount = model.PreOrder.Amount,
            ReservedAt = DateTime.UtcNow,
            //Items =new (){ new OrderItem
            //{
            //    //Id = Guid.NewGuid(),
            //    //ProductId = request.P,
            //    //Quantity = i.Quantity,
            //    //UnitPrice = 0
            //}
        };

        //await _db.Orders.AddAsync(order, cancellationToken);
        //await _db.SaveChangesAsync(cancellationToken);

        return new CreateOrderResponseDto(order.Id, order.Status, order.TotalAmount);
    }

    public async Task<CompleteOrderResponseDto> CompleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken) ?? throw new KeyNotFoundException("Order not found");

        if (!string.Equals(order.Status, "Reserved", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Order state is not Reserved");

        var res = await _balance.CompleteAsync(new CompleteRequestDto(order.Id.ToString()), cancellationToken);
        order.Status = res.Status.Equals("success", StringComparison.OrdinalIgnoreCase) ? "Paid" : "Failed";
        order.CompletedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
        return new CompleteOrderResponseDto(order.Id, order.Status);
    }
}
