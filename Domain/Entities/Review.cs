using System;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A product review written by a customer. Belongs to one <see cref="Product"/>
/// and one <see cref="Customer"/> (many-to-one on both), giving
/// Product 1 → * Review and Customer 1 → * Review.
/// </summary>
public class Review
{
    public int Id { get; set; }

    /// <summary>Star rating, 1–5.</summary>
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    /// <summary>Foreign key to the reviewed product.</summary>
    public int ProductId { get; set; }

    /// <summary>The product being reviewed (* → 1).</summary>
    public Product Product { get; set; } = null!;

    /// <summary>Foreign key to the reviewing customer.</summary>
    public int CustomerId { get; set; }

    /// <summary>The customer who wrote the review (* → 1).</summary>
    public Customer Customer { get; set; } = null!;
}
