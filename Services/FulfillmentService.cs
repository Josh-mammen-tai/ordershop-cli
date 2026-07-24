using System;
using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Fulfillment business flow. Creates a shipment for a paid order, marks it
/// shipped and later delivered, and notifies the customer at each step.
/// </summary>
public sealed class FulfillmentService
{
    private readonly IOrderRepository _orders;
    private readonly NotificationService _notifications;

    public FulfillmentService(IOrderRepository orders, NotificationService notifications)
    {
        _orders = orders;
        _notifications = notifications;
    }

    /// <summary>Ship a paid order with the given carrier. Returns null if not shippable.</summary>
    public Shipment? Ship(int orderId, string carrier)
    {
        Order? order = _orders.GetWithDetails(orderId);
        if (order is null || order.Status != OrderStatus.Paid)
        {
            return null;
        }

        Shipment shipment = new()
        {
            OrderId = order.Id,
            Carrier = carrier,
            TrackingNumber = $"TRK-{order.Id:D6}",
            Status = ShipmentStatus.Shipped,
            ShippedAt = DateTime.UtcNow,
        };

        order.Shipment = shipment;
        order.Status = OrderStatus.Shipped;
        _orders.Save();

        _notifications.OrderShipped(order.Customer, shipment);
        return shipment;
    }

    /// <summary>Mark a shipped order as delivered.</summary>
    public void MarkDelivered(int orderId)
    {
        Order? order = _orders.GetWithDetails(orderId);
        if (order?.Shipment is null)
        {
            return;
        }

        order.Shipment.Status = ShipmentStatus.Delivered;
        order.Status = OrderStatus.Delivered;
        _orders.Save();
    }
}
