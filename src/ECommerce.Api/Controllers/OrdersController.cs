using ECommerce.Application.Commands.CompleteOrder;
using ECommerce.Application.Commands.CreateOrder;
using ECommerce.Application.Queries.GetProducts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the list of products from Balance Management service.
    /// </summary>
    /// <remarks>
    /// This endpoint proxies the external Balance API's `/products` endpoint.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/api/products")]
    public async Task<ActionResult<IReadOnlyList<GetProductsQueryResult>>> GetProducts(CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetProductsQuery(), cancellationToken)); ;
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
    public async Task<ActionResult<CreateOrderCommandResult>> Create([FromBody] CreateOrderCommand command, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
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
    public async Task<ActionResult<CompleteOrderCommandResult>> Complete([FromRoute] string orderId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteOrderCommand(orderId), cancellationToken);
        return Ok(result);
    }
}
