using ECommerce.Application.Abstractions.Models;

namespace ECommerce.Application.Abstractions;

public interface IBalanceClient
{
    Task<IReadOnlyList<ProductInfo>> GetProductsAsync(CancellationToken cancellationToken);
    Task<PreorderInfo> PreorderAsync(decimal amount, string orderId, CancellationToken cancellationToken);
    Task<CompletionInfo> CompleteAsync(string orderId, CancellationToken cancellationToken);
}
