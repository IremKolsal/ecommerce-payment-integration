using ECommerce.Application.Contracts;

namespace ECommerce.Application.Services;

public interface IOrderService
{
    Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken);
    Task<CreateOrderResponseDto> CreateAsync(CreateOrderRequestDto request, CancellationToken cancellationToken);
    Task<CompleteOrderResponseDto> CompleteAsync(string orderId, CancellationToken cancellationToken);
}
