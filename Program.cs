using System;
using OrderShop.Models;
using OrderShop.Repositories;
using OrderShop.Services;

namespace OrderShop;

/// <summary>Console entry point — runs the checkout flow for a sample order.</summary>
public static class Program
{
    public static void Main()
    {
        Customer customer = new(1, "Ada Lovelace", "ada@example.com");

        Order order = new(customer);
        order.AddItem(new OrderItem("SKU-1", "USB-C Cable", 8.50m, 2));
        order.AddItem(new OrderItem("SKU-2", "Wireless Mouse", 21.00m, 1));

        // Build the collaborators for the checkout flow.
        InventoryService inventory = new();
        PricingService pricing = new();
        PaymentService payment = new();
        NotificationService notifications = new();
        OrderRepository orders = new();

        OrderService orderService = new(inventory, pricing);
        CheckoutService checkout = new(orderService, payment, notifications, orders);

        CheckoutResult result = checkout.Checkout(order, PaymentMethod.Card);

        Console.WriteLine($"Customer : {customer.Name} <{customer.Email}>");
        Console.WriteLine($"Items    : {order.Items.Count}");
        Console.WriteLine($"Status   : {result.Message}");
        Console.WriteLine($"Stored   : {orders.Count} order(s)");
    }
}
