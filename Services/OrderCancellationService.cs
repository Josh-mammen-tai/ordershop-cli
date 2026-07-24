using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Order-cancellation business flow: if the order was already paid, refund the
/// payment and restock its items; then mark the order cancelled and notify the
/// customer. Delivered or already-cancelled orders cannot be cancelled.
/// </summary>
public sealed class OrderCancellationService
{
    private readonly IOrderRepository _orders;
    private readonly PaymentService _payments;
    private readonly InventoryService _inventory;
    private readonly NotificationService _notifications;

    public OrderCancellationService(
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

    /// <summary>Cancel an order. Returns false if it is delivered or already cancelled.</summary>
    public bool Cancel(int orderId)
    {
        Order? order = _orders.GetWithDetails(orderId);
        if (order is null
            || order.Status == OrderStatus.Cancelled
            || order.Status == OrderStatus.Delivered)
        {
            return false;
        }

        if (order.Payment is not null && order.Payment.Status == PaymentStatus.Approved)
        {
            _payments.Refund(order.Payment);
            foreach (OrderItem item in order.Items)
            {
                _inventory.Restock(item.ProductId, item.Quantity);
            }
        }

        order.Status = OrderStatus.Cancelled;
        _orders.Save();

        _notifications.OrderCancelled(order.Customer, order);
        return true;
    }
}
