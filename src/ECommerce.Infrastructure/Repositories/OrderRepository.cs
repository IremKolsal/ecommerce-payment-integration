using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    private readonly AppDbContext _db = db;

    public Task<Order?> GetByExternalIdAsync(string externalOrderId, CancellationToken ct) =>
        _db.Orders.FirstOrDefaultAsync(x => x.ExternalOrderId == externalOrderId, ct);

    public Task AddAsync(Order order, CancellationToken ct) =>
        _db.Orders.AddAsync(order, ct).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
