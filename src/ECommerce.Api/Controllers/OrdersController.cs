using ECommerce.Application.Contracts;
using ECommerce.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;


    // Ürün listesi (Balance üzerinden)
    [HttpGet("/api/products")]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var products = await _service.GetProductsAsync(cancellationToken);
        return Ok(products);
    }

    // Sipariş oluşturma
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto req, CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(req, cancellationToken);
        return Ok(response);
    }

    // Siparişi tamamlama
    [HttpPost("{id:guid}/complete")]
    public async Task<IActionResult> Complete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var response = await _service.CompleteAsync(id, cancellationToken);
        return Ok(response);
    }
}
