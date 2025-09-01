namespace ECommerce.Application.Queries.GetProducts;

public sealed record GetProductsQueryResult(
    string Id,
    string Name,
    string Description,
    decimal Price,
    string Currency,
    string Category,
    int Stock
);
