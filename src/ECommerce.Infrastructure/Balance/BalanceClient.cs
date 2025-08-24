using ECommerce.Application.Contracts;
using System.Net.Http.Json;
using System.Text.Json;

namespace ECommerce.Infrastructure.Balance;

public interface IBalanceClient
{
    Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken);
    Task<PreorderResponseDto> PreorderAsync(PreorderRequestDto request, CancellationToken cancellationToken);
    Task<CompleteResponseDto> CompleteAsync(CompleteRequestDto request, CancellationToken cancellationToken);
}

public class BalanceClient(HttpClient http) : IBalanceClient
{
    public async Task<List<ProductDto>> GetProductsAsync(CancellationToken cancellationToken)
    {
        var products = await http.GetFromJsonAsync<ProductResponseModel>("products", cancellationToken);

        if (products == null)
        {
            return new List<ProductDto>();
        }

        return products.Data;
    }

    public async Task<PreorderResponseDto> PreorderAsync(PreorderRequestDto request, CancellationToken cancellationToken)
    {
        var res = await http.PostAsJsonAsync("balance/preorder", request, cancellationToken);
        res.EnsureSuccessStatusCode();

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var body = await res.Content.ReadFromJsonAsync<PreorderResponseDto>(opts, cancellationToken);

        if (body is null) throw new InvalidOperationException("Empty preorder response.");

        return body;
    }

    public async Task<CompleteResponseDto> CompleteAsync(CompleteRequestDto request, CancellationToken cancellationToken)
    {
        var res = await http.PostAsJsonAsync("balance/complete", request, cancellationToken);
        res.EnsureSuccessStatusCode();

        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var body = await res.Content.ReadFromJsonAsync<CompleteResponseDto>(opts, cancellationToken);

        if (body is null) throw new InvalidOperationException("Empty complete response.");

        return body;
    }
}
public class ProductResponseModel
{
    public bool Success { get; set; }
    public List<ProductDto> Data { get; set; }
}