namespace ECommerce.Application.Abstractions.Models;

public record ProductInfo(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string Category,
    int Stock
);
