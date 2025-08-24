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

    /// <summary>
    /// Retrieves the list of products from Balance Management service.
    /// </summary>
    /// <remarks>
    /// This endpoint proxies the external Balance API's `/products` endpoint.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/api/products")]
    public async Task<IActionResult> GetProducts(CancellationToken cancellationToken)
    {
        var products = await _service.GetProductsAsync(cancellationToken);
        return Ok(products);
    }

    /// <summary>
    /// Creates a new order and blocks balance via Balance Management service.
    /// </summary>
    /// <remarks>
    /// Calls the external `/api/balance/preorder` endpoint.  
    /// The order is persisted with status <c>blocked</c>.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _service.CreateAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Completes an existing order by calling Balance Management service.
    /// </summary>
    /// <remarks>
    /// Calls the external `/api/balance/complete` endpoint.  
    /// Changes order status to <c>completed</c> if successful.
    /// </remarks>
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{orderId}/complete")]
    public async Task<IActionResult> Complete([FromRoute] string orderId, CancellationToken cancellationToken)
    {
        var response = await _service.CompleteAsync(orderId, cancellationToken);
        return Ok(response);
    }
}
