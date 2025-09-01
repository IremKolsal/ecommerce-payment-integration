using ECommerce.Application.Abstractions;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public sealed class OrderRepository(AppDbContext db) : IOrderRepository
{
    private readonly AppDbContext _db = db;

    public Task<Order?> GetByExternalIdAsync(string externalOrderId, CancellationToken cancellationToken) =>
        _db.Orders.FirstOrDefaultAsync(x => x.ExternalOrderId == externalOrderId, cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken) =>
        _db.Orders.AddAsync(order, cancellationToken).AsTask();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken) => _db.SaveChangesAsync(cancellationToken);
}
