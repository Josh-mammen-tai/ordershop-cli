using OrderShop.Models;

namespace OrderShop.Services;

/// <summary>Orchestrates placing an order end-to-end: stock check, pricing, result.</summary>
public sealed class OrderService
{
    private readonly InventoryService _inventory;
    private readonly PricingService _pricing;

    public OrderService(InventoryService inventory, PricingService pricing)
    {
        _inventory = inventory;
        _pricing = pricing;
    }

    /// <summary>Validate stock, price the order, and return the outcome.</summary>
    public OrderResult PlaceOrder(Order order)
    {
        if (!_inventory.IsOrderFulfillable(order))
        {
            return new OrderResult(false, 0m, "One or more items are out of stock.");
        }

        decimal total = _pricing.CalculateTotal(order);
        return new OrderResult(true, total, "Order placed.");
    }
}

/// <summary>The outcome of an attempt to place an order.</summary>
public sealed class OrderResult
{
    public OrderResult(bool success, decimal total, string message)
    {
        Success = success;
        Total = total;
        Message = message;
    }

    public bool Success { get; }

    public decimal Total { get; }

    public string Message { get; }
}
