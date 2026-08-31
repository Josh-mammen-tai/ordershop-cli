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
///   <item>apply a discount code and/or loyalty points (<see cref="DiscountService"/>, <see cref="LoyaltyService"/>)</item>
///   <item>charge payment (<see cref="PaymentService"/>)</item>
///   <item>redeem loyalty points, reserve stock, persist the order (<see cref="IOrderRepository"/>)</item>
///   <item>notify the customer (<see cref="NotificationService"/>)</item>
/// </list>
/// </summary>
public sealed class CheckoutService
{
    /// <summary>Loyalty points that buy one unit of currency (100 points = 1.00).</summary>
    private const decimal PointsPerCurrencyUnit = 100m;

    private readonly IOrderRepository _orders;
    private readonly InventoryService _inventory;
    private readonly PricingService _pricing;
    private readonly PaymentService _payments;
    private readonly NotificationService _notifications;
    private readonly DiscountService _discountService;
    private readonly LoyaltyService _loyaltyService;

    public CheckoutService(
        IOrderRepository orders,
        InventoryService inventory,
        PricingService pricing,
        PaymentService payments,
        NotificationService notifications,
        DiscountService discountService,
        LoyaltyService loyaltyService)
    {
        _orders = orders;
        _inventory = inventory;
        _pricing = pricing;
        _payments = payments;
        _notifications = notifications;
        _discountService = discountService;
        _loyaltyService = loyaltyService;
    }

    /// <summary>Run the checkout flow for a built order and chosen payment method.</summary>
    public CheckoutResult Checkout(Order order, PaymentMethod method, string? discountCode = null, int? loyaltyPoints = null)
    {
        if (!_inventory.IsAvailable(order))
        {
            return new CheckoutResult(false, "One or more items are out of stock.");
        }

        decimal total = _pricing.Total(order.Items);

        if (!string.IsNullOrEmpty(discountCode) &&
            _discountService.ValidateDiscountCode(discountCode, out DiscountCode? discount))
        {
            total = _discountService.ApplyDiscount(total, discount);
        }

        if (loyaltyPoints.HasValue && loyaltyPoints.Value < 0)
        {
            return new CheckoutResult(false, "Loyalty points cannot be negative.");
        }

        // Work out the loyalty spend now, but redeem it only once payment is approved:
        // points must survive a declined payment.
        int pointsToRedeem = 0;
        if (loyaltyPoints.HasValue && loyaltyPoints.Value > 0)
        {
            int requested = loyaltyPoints.Value;
            if (requested > _loyaltyService.GetPointsBalance(order.CustomerId))
            {
                return new CheckoutResult(false, "Not enough loyalty points for this redemption.");
            }

            // The discount stops at zero, so spending past the order total would burn
            // points for nothing. Redeem only what the order can actually absorb.
            // Compared as decimal: a large total would overflow an int conversion, and
            // the narrowing cast is only reached once it is known to be below requested.
            decimal redeemable = Math.Floor(total * PointsPerCurrencyUnit);
            pointsToRedeem = redeemable >= requested ? requested : (int)redeemable;
            total -= pointsToRedeem / PointsPerCurrencyUnit;
        }

        // A total of zero means points covered the order outright — there is nothing
        // left for the gateway to charge, so settle it directly.
        Payment payment = total <= 0m && pointsToRedeem > 0
            ? _payments.SettleWithPoints(order, method)
            : _payments.Charge(order, total, method);

        if (payment.Status != PaymentStatus.Approved)
        {
            return new CheckoutResult(false, "Payment was declined.");
        }

        // Payment succeeded — only now is it safe to spend the customer's points.
        if (pointsToRedeem > 0)
        {
            _loyaltyService.RedeemPoints(order.CustomerId, pointsToRedeem);
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
