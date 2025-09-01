namespace ECommerce.Application.Abstractions.Models;

public sealed record CompletionInfo(
    string ExternalOrderId,
    string Status,
    DateTime? CompletedAt
);
