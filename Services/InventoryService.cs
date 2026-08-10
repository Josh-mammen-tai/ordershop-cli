using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Stock availability, reservation, and restock — the inventory side of the
/// checkout and refund flows. Backed by the product repository.
/// </summary>
public sealed class InventoryService
{
    private readonly IProductRepository _products;

    public InventoryService(IProductRepository products)
    {
        _products = products;
    }

    /// <summary>True only if every line in the order can be fulfilled from stock.</summary>
    public bool IsAvailable(Order order)
    {
        foreach (OrderItem item in order.Items)
        {
            Product? product = _products.GetById(item.ProductId);
            if (product is null || product.StockQuantity < item.Quantity)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Reserve (decrement) stock for every line in the order.</summary>
    public void Reserve(Order order)
    {
        foreach (OrderItem item in order.Items)
        {
            _products.AdjustStock(item.ProductId, -item.Quantity);
        }

        _products.Save();
    }

    /// <summary>Return stock to the shelf (used by the refund flow).</summary>
    public void Restock(int productId, int quantity)
    {
        _products.AdjustStock(productId, quantity);
        _products.Save();
    }
}
