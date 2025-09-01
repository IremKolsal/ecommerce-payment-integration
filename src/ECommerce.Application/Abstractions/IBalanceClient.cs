using ECommerce.Application.Abstractions.Models;

namespace ECommerce.Application.Abstractions;

public interface IBalanceClient
{
    Task<IReadOnlyList<ProductInfo>> GetProductsAsync(CancellationToken cancellationToken);
    Task<PreorderInfo> PreorderAsync(int amount, string orderId, CancellationToken cancellationToken);
    Task<CompletionInfo> CompleteAsync(string orderId, CancellationToken cancellationToken);
}
