using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;
using OrderShop.Services;

namespace OrderShop.Controllers;

/// <summary>HTTP entry points for reading orders and driving their lifecycle flows.</summary>
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IOrderRepository _orders;
    private readonly FulfillmentService _fulfillment;
    private readonly RefundService _refunds;
    private readonly OrderCancellationService _cancellations;

    public OrdersController(
        IOrderRepository orders,
        FulfillmentService fulfillment,
        RefundService refunds,
        OrderCancellationService cancellations)
    {
        _orders = orders;
        _fulfillment = fulfillment;
        _refunds = refunds;
        _cancellations = cancellations;
    }

    /// <summary>Fetch an order with all related entities.</summary>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        Order? order = _orders.GetWithDetails(id);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>Ship a paid order (fulfillment flow).</summary>
    [HttpPost("{id:int}/ship")]
    public IActionResult Ship(int id, [FromQuery] string carrier)
    {
        Shipment? shipment = _fulfillment.Ship(id, carrier);
        return shipment is null ? BadRequest("Order is not ready to ship.") : Ok(shipment);
    }

    /// <summary>Mark a shipped order as delivered (fulfillment flow).</summary>
    [HttpPost("{id:int}/deliver")]
    public IActionResult Deliver(int id)
    {
        _fulfillment.MarkDelivered(id);
        return Ok();
    }

    /// <summary>Cancel an order — refund + restock if paid (cancellation flow).</summary>
    [HttpPost("{id:int}/cancel")]
    public IActionResult Cancel(int id)
    {
        return _cancellations.Cancel(id) ? Ok() : BadRequest("Order cannot be cancelled.");
    }

    /// <summary>Refund a delivered/paid order (refund flow).</summary>
    [HttpPost("{id:int}/refund")]
    public IActionResult Refund(int id)
    {
        return _refunds.Refund(id) ? Ok() : BadRequest("Order cannot be refunded.");
    }
}
