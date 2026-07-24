using System.Collections.Generic;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>Prices an order's line items into a subtotal, tax, and grand total.</summary>
public sealed class PricingService
{
    private const decimal TaxRate = 0.08m;

    /// <summary>Sum of every line total, before tax.</summary>
    public decimal Subtotal(IEnumerable<OrderItem> items)
    {
        decimal sum = 0m;
        foreach (OrderItem item in items)
        {
            sum += item.LineTotal;
        }

        return sum;
    }

    /// <summary>Tax charged on a subtotal.</summary>
    public decimal Tax(decimal subtotal)
    {
        return subtotal * TaxRate;
    }

    /// <summary>The amount the customer pays: subtotal plus tax.</summary>
    public decimal Total(IEnumerable<OrderItem> items)
    {
        decimal subtotal = Subtotal(items);
        return subtotal + Tax(subtotal);
    }
}
