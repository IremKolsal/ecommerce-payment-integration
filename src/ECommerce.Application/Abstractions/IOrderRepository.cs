using ECommerce.Domain.Entities;

namespace ECommerce.Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetByExternalIdAsync(string externalOrderId, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
