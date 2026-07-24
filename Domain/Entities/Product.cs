using System.Collections.Generic;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A catalog product. Belongs to one <see cref="Category"/> (many-to-one) and is
/// referenced by many <see cref="OrderItem"/>s (one-to-many).
/// </summary>
public class Product
{
    public int Id { get; set; }

    public string Sku { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    /// <summary>Foreign key to the owning category.</summary>
    public int CategoryId { get; set; }

    /// <summary>The category this product belongs to (* → 1).</summary>
    public Category Category { get; set; } = null!;

    /// <summary>Order lines that reference this product (1 → *).</summary>
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
