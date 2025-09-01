namespace ECommerce.Application.Abstractions.Models;

public sealed record PreorderInfo(
    string ExternalOrderId,
    decimal Amount,
    string Status
);
