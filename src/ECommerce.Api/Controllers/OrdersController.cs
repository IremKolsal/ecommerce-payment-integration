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
    /// <param name="command"></param>
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
    /// <param name="orderId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{orderId}/complete")]
    public async Task<ActionResult<CompleteOrderCommandResult>> Complete([FromRoute] string orderId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new CompleteOrderCommand(orderId), cancellationToken));
    }
}
