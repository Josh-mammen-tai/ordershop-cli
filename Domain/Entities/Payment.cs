using System;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A payment for an order. One-to-one with <see cref="Order"/>: each order has at
/// most one payment, and each payment settles exactly one order.
/// </summary>
public class Payment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string Reference { get; set; } = string.Empty;

    public DateTime? ProcessedAt { get; set; }

    /// <summary>Foreign key to the settled order.</summary>
    public int OrderId { get; set; }

    /// <summary>The order this payment settles (1 → 1).</summary>
    public Order Order { get; set; } = null!;
}
