using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByExternalIdAsync(string externalOrderId, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task<int> SaveChangesAsync(CancellationToken ct);
}
