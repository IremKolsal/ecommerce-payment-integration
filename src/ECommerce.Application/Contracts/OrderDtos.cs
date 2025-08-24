namespace ECommerce.Application.Contracts;

public record CreateOrderItemDto(string ProductId, int Quantity);
public record CreateOrderRequestDto(int Amount, string OrderId);
public record CreateOrderResponseDto(Guid Id, string OrderId, string Status, decimal TotalAmount);
public record CompleteOrderResponseDto(string OrderId, string Status);
