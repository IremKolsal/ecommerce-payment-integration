namespace ECommerce.Application.Abstractions.Models;

public sealed record PreorderInfo(
    string OrderId,
    int Amount,
    string Status
);
