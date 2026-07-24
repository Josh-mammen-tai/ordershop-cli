namespace OrderShop.Domain.Entities;

/// <summary>
/// A single line in an order — the join between <see cref="Order"/> and
/// <see cref="Product"/> (each order has many items; each product appears on many
/// items), carrying the quantity and the price captured at purchase time.
/// </summary>
public class OrderItem
{
    public int Id { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    /// <summary>Foreign key to the owning order.</summary>
    public int OrderId { get; set; }

    /// <summary>The order this line belongs to (* → 1).</summary>
    public Order Order { get; set; } = null!;

    /// <summary>Foreign key to the ordered product.</summary>
    public int ProductId { get; set; }

    /// <summary>The product on this line (* → 1).</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Price for this line (unit price × quantity).</summary>
    public decimal LineTotal => UnitPrice * Quantity;
}
