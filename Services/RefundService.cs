using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Refund business flow. Refunds the payment, restocks each item, marks the order
/// refunded, and notifies the customer.
/// </summary>
public sealed class RefundService
{
    private readonly IOrderRepository _orders;
    private readonly PaymentService _payments;
    private readonly InventoryService _inventory;
    private readonly NotificationService _notifications;

    public RefundService(
        IOrderRepository orders,
        PaymentService payments,
        InventoryService inventory,
        NotificationService notifications)
    {
        _orders = orders;
        _payments = payments;
        _inventory = inventory;
        _notifications = notifications;
    }

    /// <summary>Refund an order. Returns false if it has no payment or is already refunded.</summary>
    public bool Refund(int orderId)
    {
        Order? order = _orders.GetWithDetails(orderId);
        if (order?.Payment is null || order.Status == OrderStatus.Refunded)
        {
            return false;
        }

        _payments.Refund(order.Payment);

        foreach (OrderItem item in order.Items)
        {
            _inventory.Restock(item.ProductId, item.Quantity);
        }

        order.Status = OrderStatus.Refunded;
        _orders.Save();

        _notifications.OrderRefunded(order.Customer, order);
        return true;
    }
}
