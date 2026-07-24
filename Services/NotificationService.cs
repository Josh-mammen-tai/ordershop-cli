using System;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>Sends customer notifications for the order lifecycle (console-backed).</summary>
public sealed class NotificationService
{
    public void Welcome(Customer customer)
    {
        Console.WriteLine($"[notify] {customer.Email}: welcome to OrderShop, {customer.Name}!");
    }

    public void OrderConfirmed(Customer customer, Order order)
    {
        Console.WriteLine($"[notify] {customer.Email}: order #{order.Id} confirmed.");
    }

    public void OrderShipped(Customer customer, Shipment shipment)
    {
        Console.WriteLine(
            $"[notify] {customer.Email}: order #{shipment.OrderId} shipped via " +
            $"{shipment.Carrier} ({shipment.TrackingNumber}).");
    }

    public void OrderCancelled(Customer customer, Order order)
    {
        Console.WriteLine($"[notify] {customer.Email}: order #{order.Id} cancelled.");
    }

    public void OrderRefunded(Customer customer, Order order)
    {
        Console.WriteLine($"[notify] {customer.Email}: order #{order.Id} refunded.");
    }
}
