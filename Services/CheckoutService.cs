using OrderShop.Models;
using OrderShop.Repositories;

namespace OrderShop.Services;

/// <summary>
/// The end-to-end checkout business flow. Ties the whole app together:
///   1. validate stock and price the order  (OrderService)
///   2. take payment for the total          (PaymentService)
///   3. persist the order                    (OrderRepository)
///   4. notify the customer                  (NotificationService)
/// </summary>
public sealed class CheckoutService
{
    private readonly OrderService _orderService;
    private readonly PaymentService _paymentService;
    private readonly NotificationService _notificationService;
    private readonly OrderRepository _orderRepository;

    public CheckoutService(
        OrderService orderService,
        PaymentService paymentService,
        NotificationService notificationService,
        OrderRepository orderRepository)
    {
        _orderService = orderService;
        _paymentService = paymentService;
        _notificationService = notificationService;
        _orderRepository = orderRepository;
    }

    /// <summary>Run the full checkout flow for an order and a chosen payment method.</summary>
    public CheckoutResult Checkout(Order order, PaymentMethod method)
    {
        // 1. Validate stock and price the order.
        OrderResult placed = _orderService.PlaceOrder(order);
        if (!placed.Success)
        {
            return new CheckoutResult(false, placed.Message);
        }

        // 2. Take payment for the priced total.
        Payment payment = new(placed.Total, method);
        PaymentResult paid = _paymentService.Charge(payment);
        if (!paid.Approved)
        {
            return new CheckoutResult(false, paid.Message);
        }

        // 3. Persist the order and notify the customer.
        _orderRepository.Save(order);
        _notificationService.SendOrderConfirmation(order.Customer, paid.Reference);

        return new CheckoutResult(true, $"Checkout complete. {paid.Reference}");
    }
}

/// <summary>The outcome of a checkout attempt.</summary>
public sealed class CheckoutResult
{
    public CheckoutResult(bool success, string message)
    {
        Success = success;
        Message = message;
    }

    public bool Success { get; }

    public string Message { get; }
}
