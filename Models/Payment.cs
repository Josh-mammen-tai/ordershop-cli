namespace OrderShop.Models;

/// <summary>The ways a customer can pay for an order.</summary>
public enum PaymentMethod
{
    Card,
    PayPal,
    BankTransfer,
}

/// <summary>A payment attempt for a given amount and method.</summary>
public sealed class Payment
{
    public Payment(decimal amount, PaymentMethod method)
    {
        Amount = amount;
        Method = method;
    }

    public decimal Amount { get; }

    public PaymentMethod Method { get; }
}

/// <summary>The outcome of charging a <see cref="Payment"/>.</summary>
public sealed class PaymentResult
{
    public PaymentResult(bool approved, string reference, string message)
    {
        Approved = approved;
        Reference = reference;
        Message = message;
    }

    public bool Approved { get; }

    public string Reference { get; }

    public string Message { get; }
}
