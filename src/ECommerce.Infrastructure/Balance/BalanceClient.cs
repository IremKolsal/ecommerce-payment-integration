using System.Net.Http.Json;
using ECommerce.Application.Contracts;

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
        try
        {
            var products = await http.GetFromJsonAsync<ProductResponseModel>("products", cancellationToken);

            if (products == null)
            {
                return new List<ProductDto>();
            }

            return products.Data;
        }
        catch (Exception e)
        {

            throw e;
        }
    }

    public async Task<PreorderResponseDto> PreorderAsync(PreorderRequestDto request, CancellationToken cancellationToken)
    {
        var res = await http.PostAsJsonAsync("preorder", request, cancellationToken);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<PreorderResponseDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<CompleteResponseDto> CompleteAsync(CompleteRequestDto request, CancellationToken cancellationToken)
    {
        var res = await http.PostAsJsonAsync("complete", request, cancellationToken);
        res.EnsureSuccessStatusCode();
        return (await res.Content.ReadFromJsonAsync<CompleteResponseDto>(cancellationToken: cancellationToken))!;
    }
}
 public class ProductResponseModel
{
    public bool Success { get; set; }
    public List<ProductDto> Data { get; set; }
}