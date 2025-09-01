using AutoMapper;
using ECommerce.Application.Abstractions;
using MediatR;

namespace ECommerce.Application.Queries.GetProducts;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IReadOnlyList<GetProductsQueryResult>>
{
    private readonly IBalanceClient _balance;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IBalanceClient balance, IMapper mapper)
    {
        _balance = balance;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<GetProductsQueryResult>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _balance.GetProductsAsync(cancellationToken);
        var results = _mapper.Map<List<GetProductsQueryResult>>(products);
        return results;
    }
}
