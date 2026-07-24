using System;
using System.Collections.Generic;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A customer order — the aggregate root of the domain. It belongs to one
/// <see cref="Customer"/>, ships to one <see cref="Address"/>, has many
/// <see cref="OrderItem"/>s, and has one <see cref="Payment"/> and one
/// <see cref="Shipment"/> (both one-to-one).
/// </summary>
public class Order
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>Foreign key to the owning customer.</summary>
    public int CustomerId { get; set; }

    /// <summary>The customer who placed the order (* → 1).</summary>
    public Customer Customer { get; set; } = null!;

    /// <summary>Foreign key to the shipping address.</summary>
    public int ShippingAddressId { get; set; }

    /// <summary>Where the order ships to (* → 1).</summary>
    public Address ShippingAddress { get; set; } = null!;

    /// <summary>The order's line items (1 → *).</summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

    /// <summary>The payment for this order (1 → 1, optional until paid).</summary>
    public Payment? Payment { get; set; }

    /// <summary>The shipment for this order (1 → 1, optional until shipped).</summary>
    public Shipment? Shipment { get; set; }
}
