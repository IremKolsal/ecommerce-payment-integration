using System.Text.Json.Serialization;

namespace ECommerce.Infrastructure.Balance.Models;

internal sealed record CompleteRequestDto(
    [property: JsonPropertyName("orderId")] string OrderId
);

internal sealed record CompleteData(
    CompletedOrder Order,
    UpdatedBalance UpdatedBalance
);

internal sealed record CompletedOrder(
    [property: JsonPropertyName("orderId")] string OrderId,
    decimal Amount,
    DateTime Timestamp,
    string Status,
    DateTime? CompletedAt
);
