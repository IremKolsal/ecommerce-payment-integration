using System.Text.Json.Serialization;

namespace ECommerce.Application.Contracts;

public record ProductDto(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string Category,
    int Stock
);

public record PreorderRequestDto(
    int Amount,
    string OrderId
);

public record PreorderResponseDto(
    bool Success,
    string Message,
    PreorderData Data
);

public record PreorderData(
    PreOrderDetail PreOrder,
    UpdatedBalance UpdatedBalance
);

public record PreOrderDetail(
    string OrderId,
    decimal Amount,
    DateTime Timestamp,
    string Status,
    DateTime? CompletedAt,
    DateTime? CancelledAt
);

public record UpdatedBalance(
    string UserId,
    decimal TotalBalance,
    decimal AvailableBalance,
    decimal BlockedBalance,
    string Currency,
    DateTime LastUpdated
);

public record CompleteRequestDto(
    [property: JsonPropertyName("orderId")] string OrderId
);

public record CompleteResponseDto(
    bool Success,
    string Message,
    CompleteData Data
);

public record CompleteData(
    CompletedOrder Order,
    UpdatedBalance UpdatedBalance
);

public record CompletedOrder(
    [property: JsonPropertyName("orderId")] string OrderId,
    decimal Amount,
    DateTime Timestamp,
    string Status,
    DateTime? CompletedAt
);

