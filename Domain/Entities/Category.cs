using System.Collections.Generic;

namespace OrderShop.Domain.Entities;

/// <summary>A product category. One category has many <see cref="Product"/>s (1 → *).</summary>
public class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    /// <summary>Products in this category (1 → *).</summary>
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
