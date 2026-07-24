namespace OrderShop.Domain;

/// <summary>Lifecycle status of an <see cref="Entities.Order"/>.</summary>
public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Delivered,
    Cancelled,
    Refunded,
}

/// <summary>How a customer pays for an order.</summary>
public enum PaymentMethod
{
    Card,
    PayPal,
    BankTransfer,
}

/// <summary>Result state of a payment attempt.</summary>
public enum PaymentStatus
{
    Pending,
    Approved,
    Declined,
    Refunded,
}

/// <summary>Progress of an order's shipment.</summary>
public enum ShipmentStatus
{
    Preparing,
    Shipped,
    InTransit,
    Delivered,
}
