using MediatR;

namespace ECommerce.Application.Queries.GetProducts;

public sealed record GetProductsQuery() : IRequest<IReadOnlyList<GetProductsQueryResult>>;

