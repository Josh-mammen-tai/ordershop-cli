using System.Collections.Generic;
using OrderShop.Models;

namespace OrderShop.Services;

/// <summary>A very small in-memory stock lookup.</summary>
public sealed class InventoryService
{
    private readonly Dictionary<string, int> _stock = new()
    {
        ["SKU-1"] = 25,
        ["SKU-2"] = 4,
        ["SKU-3"] = 0,
    };

    /// <summary>Is <paramref name="quantity"/> of <paramref name="sku"/> available?</summary>
    public bool IsInStock(string sku, int quantity)
        => _stock.TryGetValue(sku, out int onHand) && onHand >= quantity;

    /// <summary>True only if every item in the order can be fulfilled.</summary>
    public bool IsOrderFulfillable(Order order)
    {
        foreach (OrderItem item in order.Items)
        {
            if (!IsInStock(item.Sku, item.Quantity))
            {
                return false;
            }
        }

        return true;
    }
}
