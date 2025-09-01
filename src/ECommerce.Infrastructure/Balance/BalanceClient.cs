using AutoMapper;
using ECommerce.Application.Abstractions;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Infrastructure.Balance.Models;
using ECommerce.Infrastructure.Common.Errors;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.Infrastructure.Balance;

public class BalanceClient : IBalanceClient
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private readonly HttpClient _http;
    private readonly IMapper _mapper;

    public BalanceClient(HttpClient http, IMapper mapper)
    {
        _http = http;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ProductInfo>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var env = await _http.GetFromJsonAsync<ResponseEnvelope<List<ProductDto>>>(Endpoints.Products, _json, cancellationToken)
                  ?? throw new EmptyResponseException(Endpoints.Products);

        ResponseGuard.EnsureOk(env, Endpoints.Products);

        var data = env.Data ?? new List<ProductDto>();
        return _mapper.Map<List<ProductInfo>>(data).ToArray();
    }

    public async Task<PreorderInfo> PreorderAsync(int amount, string orderId, CancellationToken cancellationToken)
    {

        var req = new PreorderRequestDto(amount, orderId);
        using var res = await _http.PostAsJsonAsync(Endpoints.Preorder, req, _json, cancellationToken);
        res.EnsureSuccessStatusCode();

        var env = await res.Content.ReadFromJsonAsync<ResponseEnvelope<PreorderData>>(_json, cancellationToken)
                  ?? throw new EmptyResponseException(Endpoints.Preorder);

        ResponseGuard.EnsureOk(env, Endpoints.Preorder);

        var pre = ResponseGuard.ThrowIfNull(env.Data, $"{Endpoints.Preorder}: data").PreOrder;
        return _mapper.Map<PreorderInfo>(pre);
    }

    public async Task<CompletionInfo> CompleteAsync(string orderId, CancellationToken cancellationToken)
    {
        var req = new CompleteRequestDto(orderId);
        using var res = await _http.PostAsJsonAsync(Endpoints.Complete, req, _json, cancellationToken);
        res.EnsureSuccessStatusCode();

        var env = await res.Content.ReadFromJsonAsync<ResponseEnvelope<CompleteData>>(_json, cancellationToken)
                  ?? throw new EmptyResponseException(Endpoints.Complete);

        ResponseGuard.EnsureOk(env, Endpoints.Complete);

        var ord = ResponseGuard.ThrowIfNull(env.Data, $"{Endpoints.Complete}: data").Order;
        return _mapper.Map<CompletionInfo>(ord);
    }
}