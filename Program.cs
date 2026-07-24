using System;
using OrderShop.Models;
using OrderShop.Services;

namespace OrderShop;

/// <summary>Console entry point — builds a sample order and prints a receipt.</summary>
public static class Program
{
    public static void Main()
    {
        Customer customer = new(1, "Ada Lovelace", "ada@example.com");

        Order order = new(customer);
        order.AddItem(new OrderItem("SKU-1", "USB-C Cable", 8.50m, 2));
        order.AddItem(new OrderItem("SKU-2", "Wireless Mouse", 21.00m, 1));

        InventoryService inventory = new();
        PricingService pricing = new();
        OrderService orderService = new(inventory, pricing);

        OrderResult result = orderService.PlaceOrder(order);

        Console.WriteLine($"Customer : {customer.Name} <{customer.Email}>");
        Console.WriteLine($"Items    : {order.Items.Count}");
        Console.WriteLine($"Total    : {result.Total:C}");
        Console.WriteLine($"Status   : {result.Message}");
    }
}
