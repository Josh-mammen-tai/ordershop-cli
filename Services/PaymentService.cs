using OrderShop.Models;

namespace OrderShop.Services;

/// <summary>Charges payments for an order total (a simulated payment gateway).</summary>
public sealed class PaymentService
{
    private const decimal MaxCardAmount = 5000m;

    /// <summary>Attempt to charge <paramref name="payment"/> and return an approval result.</summary>
    public PaymentResult Charge(Payment payment)
    {
        if (payment.Amount <= 0m)
        {
            return new PaymentResult(false, string.Empty, "Amount must be positive.");
        }

        if (payment.Method == PaymentMethod.Card && payment.Amount > MaxCardAmount)
        {
            return new PaymentResult(false, string.Empty, "Card limit exceeded.");
        }

        string reference = $"PAY-{payment.Method}-{payment.Amount:0.00}";
        return new PaymentResult(true, reference, "Payment approved.");
    }
}
