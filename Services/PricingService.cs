using OrderShop.Models;

namespace OrderShop.Services;

/// <summary>Calculates the monetary totals for an order.</summary>
public sealed class PricingService
{
    /// <summary>Sum of every line total in the order, before any adjustments.</summary>
    public decimal Subtotal(Order order)
    {
        decimal sum = 0m;
        foreach (OrderItem item in order.Items)
        {
            sum += item.LineTotal;
        }

        return sum;
    }

    /// <summary>The amount the customer pays for the order.</summary>
    public decimal CalculateTotal(Order order)
    {
        return Subtotal(order);
    }
}
