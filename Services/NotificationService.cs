using System;
using OrderShop.Models;

namespace OrderShop.Services;

/// <summary>Sends order confirmations to the customer (console-backed).</summary>
public sealed class NotificationService
{
    /// <summary>Notify the customer that their order was confirmed.</summary>
    public void SendOrderConfirmation(Customer customer, string paymentReference)
    {
        Console.WriteLine($"[notify] {customer.Email}: your order is confirmed ({paymentReference}).");
    }
}
