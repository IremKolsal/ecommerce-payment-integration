namespace ECommerce.Infrastructure.Balance.Models;

internal sealed record PreorderRequestDto(
    decimal Amount,
    string OrderId
);

internal sealed record PreorderData(
    PreOrderDetail PreOrder,
    UpdatedBalance UpdatedBalance
);

internal sealed record PreOrderDetail(
    string OrderId,
    decimal Amount,
    DateTime Timestamp,
    string Status,
    DateTime? CompletedAt,
    DateTime? CancelledAt
);

internal sealed record UpdatedBalance(
    string UserId,
    decimal TotalBalance,
    decimal AvailableBalance,
    decimal BlockedBalance,
    string Currency,
    DateTime LastUpdated
);
