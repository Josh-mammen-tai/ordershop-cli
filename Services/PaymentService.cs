using System;
using OrderShop.Domain;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>Charges and refunds payments through a simulated payment gateway.</summary>
public sealed class PaymentService
{
    private const decimal MaxCardAmount = 5000m;

    /// <summary>Charge <paramref name="amount"/> to the order and return the resulting payment.</summary>
    public Payment Charge(Order order, decimal amount, PaymentMethod method)
    {
        bool approved = amount > 0m && !(method == PaymentMethod.Card && amount > MaxCardAmount);

        return new Payment
        {
            OrderId = order.Id,
            Amount = amount,
            Method = method,
            Status = approved ? PaymentStatus.Approved : PaymentStatus.Declined,
            Reference = approved ? $"PAY-{method}-{amount:0.00}" : string.Empty,
            ProcessedAt = DateTime.UtcNow,
        };
    }

    /// <summary>
    /// Settle an order that loyalty points already cover in full. No money changes hands,
    /// so this records an approved zero-value payment rather than sending a zero-amount
    /// charge to the gateway (which rejects non-positive amounts).
    /// </summary>
    public Payment SettleWithPoints(Order order, PaymentMethod method)
    {
        return new Payment
        {
            OrderId = order.Id,
            Amount = 0m,
            Method = method,
            Status = PaymentStatus.Approved,
            Reference = $"PAY-POINTS-{order.Id:D6}",
            ProcessedAt = DateTime.UtcNow,
        };
    }

    /// <summary>Mark a previously approved payment as refunded.</summary>
    public void Refund(Payment payment)
    {
        payment.Status = PaymentStatus.Refunded;
        payment.ProcessedAt = DateTime.UtcNow;
    }
}
