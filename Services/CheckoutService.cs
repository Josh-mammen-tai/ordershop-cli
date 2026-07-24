using System;
using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Checkout business flow. End-to-end:
/// <list type="number">
///   <item>validate stock (<see cref="InventoryService"/>)</item>
///   <item>price the order (<see cref="PricingService"/>)</item>
///   <item>charge payment (<see cref="PaymentService"/>)</item>
///   <item>reserve stock, persist the order (<see cref="IOrderRepository"/>)</item>
///   <item>notify the customer (<see cref="NotificationService"/>)</item>
/// </list>
/// </summary>
public sealed class CheckoutService
{
    private readonly IOrderRepository _orders;
    private readonly InventoryService _inventory;
    private readonly PricingService _pricing;
    private readonly PaymentService _payments;
    private readonly NotificationService _notifications;

    public CheckoutService(
        IOrderRepository orders,
        InventoryService inventory,
        PricingService pricing,
        PaymentService payments,
        NotificationService notifications)
    {
        _orders = orders;
        _inventory = inventory;
        _pricing = pricing;
        _payments = payments;
        _notifications = notifications;
    }

    /// <summary>Run the checkout flow for a built order and chosen payment method.</summary>
    public CheckoutResult Checkout(Order order, PaymentMethod method)
    {
        if (!_inventory.IsAvailable(order))
        {
            return new CheckoutResult(false, "One or more items are out of stock.");
        }

        decimal total = _pricing.Total(order.Items);
        Payment payment = _payments.Charge(order, total, method);
        if (payment.Status != PaymentStatus.Approved)
        {
            return new CheckoutResult(false, "Payment was declined.");
        }

        _inventory.Reserve(order);

        order.Payment = payment;
        order.Status = OrderStatus.Paid;
        order.CreatedAt = DateTime.UtcNow;

        _orders.Add(order);
        _orders.Save();

        _notifications.OrderConfirmed(order.Customer, order);
        return new CheckoutResult(true, payment.Reference);
    }
}

/// <summary>The outcome of a checkout attempt.</summary>
public sealed class CheckoutResult
{
    public CheckoutResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }

    public string Message { get; }
}
