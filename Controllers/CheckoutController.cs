using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;
using OrderShop.Services;

namespace OrderShop.Controllers;

/// <summary>HTTP entry point for the checkout business flow.</summary>
[ApiController]
[Route("api/checkout")]
public sealed class CheckoutController : ControllerBase
{
    private readonly CheckoutService _checkout;
    private readonly ICustomerRepository _customers;

    public CheckoutController(CheckoutService checkout, ICustomerRepository customers)
    {
        _checkout = checkout;
        _customers = customers;
    }

    /// <summary>
    /// Check out an order for a customer using the given payment method, optionally
    /// applying a discount code and/or redeeming loyalty points against the total.
    /// </summary>
    [HttpPost("{customerId:int}")]
    public IActionResult Checkout(
        int customerId,
        [FromBody] Order order,
        [FromQuery] PaymentMethod method,
        [FromQuery] string? discountCode = null,
        [FromQuery] int? loyaltyPoints = null)
    {
        Customer? customer = _customers.GetWithOrders(customerId);
        if (customer is null)
        {
            return NotFound($"Customer {customerId} not found.");
        }

        order.Customer = customer;
        order.CustomerId = customer.Id;

        CheckoutResult result = _checkout.Checkout(order, method, discountCode, loyaltyPoints);
        return result.Success
            ? Ok(new { reference = result.Message })
            : BadRequest(result.Message);
    }
}
