using AutoMapper;
using ECommerce.Application.Abstractions;
using ECommerce.Application.Abstractions.Models;
using ECommerce.Infrastructure.Balance.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.Infrastructure.Balance;

public class BalanceClient : IBalanceClient
{
    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
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
        var envelope = await _http.GetFromJsonAsync<ApiEnvelope<List<ProductDto>>>(
                           Endpoints.Products, _json, cancellationToken)
                       ?? throw new InvalidOperationException("Empty products response.");

        if (!envelope.Success)
            throw new InvalidOperationException($"Upstream error: {envelope.Message}");

        var data = envelope.Data ?? new List<ProductDto>();

        return _mapper.Map<List<ProductInfo>>(data).ToArray();
    }

    public async Task<PreorderInfo> PreorderAsync(decimal amount, string orderId, CancellationToken cancellationToken)
    {
        var req = new PreorderRequestDto(amount, orderId);

        using var res = await _http.PostAsJsonAsync(Endpoints.Preorder, req, _json, cancellationToken);

        res.EnsureSuccessStatusCode();

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<PreorderData>>(_json, cancellationToken)
                  ?? throw new InvalidOperationException("Empty preorder response.");

        if (!env.Success)
            throw new InvalidOperationException($"Upstream error: {env.Message}");

        var pre = env.Data?.PreOrder
                  ?? throw new InvalidOperationException("Preorder payload missing.");

        return _mapper.Map<PreorderInfo>(pre);
    }

    public async Task<CompletionInfo> CompleteAsync(string orderId, CancellationToken cancellationToken)
    {
        var req = new CompleteRequestDto(orderId);

        using var res = await _http.PostAsJsonAsync(Endpoints.Complete, req, _json, cancellationToken);
        res.EnsureSuccessStatusCode();

        var env = await res.Content.ReadFromJsonAsync<ApiEnvelope<CompleteData>>(_json, cancellationToken)
                  ?? throw new InvalidOperationException("Empty complete response.");

        if (!env.Success)
            throw new InvalidOperationException($"Upstream error: {env.Message}");

        var ord = env.Data?.Order
                  ?? throw new InvalidOperationException("Complete payload missing.");

        return _mapper.Map<CompletionInfo>(ord);
    }
}