using System;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A shipment for an order. One-to-one with <see cref="Order"/>: each order has at
/// most one shipment, created once the order is paid and fulfilled.
/// </summary>
public class Shipment
{
    public int Id { get; set; }

    public string Carrier { get; set; } = string.Empty;

    public string TrackingNumber { get; set; } = string.Empty;

    public ShipmentStatus Status { get; set; } = ShipmentStatus.Preparing;

    public DateTime? ShippedAt { get; set; }

    /// <summary>Foreign key to the shipped order.</summary>
    public int OrderId { get; set; }

    /// <summary>The order this shipment fulfils (1 → 1).</summary>
    public Order Order { get; set; } = null!;
}
