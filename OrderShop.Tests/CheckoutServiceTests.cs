using System;
using System.Collections.Generic;
using System.Linq;
using OrderShop.Data.Repositories;
using OrderShop.Domain;
using OrderShop.Domain.Entities;
using OrderShop.Services;
using Xunit;

namespace OrderShop.Tests;

/// <summary>
/// Covers the loyalty-points leg of the checkout flow — in particular that points are
/// only ever spent once payment has been approved.
/// </summary>
public sealed class CheckoutServiceTests
{
    [Fact]
    public void Checkout_PaymentDeclined_LeavesLoyaltyPointsUntouched()
    {
        // 1 x 10000.00 + 8% tax = 10800.00, which is over the card limit and declines.
        Fixture fixture = new(unitPrice: 10000m, stock: 5, loyaltyPoints: 500);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: 500);

        Assert.False(result.Success);
        Assert.Equal("Payment was declined.", result.Message);

        // The regression: points used to be redeemed before the charge, so a decline
        // silently cost the customer their whole balance.
        Assert.Equal(500, fixture.Customer.LoyaltyPoints);
        Assert.Empty(fixture.History.Entries);
        Assert.Empty(fixture.Orders.Added);
    }

    [Fact]
    public void Checkout_PartialRedemption_ReducesTotalAndDebitsPoints()
    {
        // 1 x 100.00 + 8% tax = 108.00; 1000 points are worth 10.00.
        Fixture fixture = new(unitPrice: 100m, stock: 5, loyaltyPoints: 5000);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: 1000);

        Assert.True(result.Success);
        Assert.Equal(98m, fixture.ChargedAmount);
        Assert.Equal(4000, fixture.Customer.LoyaltyPoints);

        LoyaltyPointsHistory entry = Assert.Single(fixture.History.Entries);
        Assert.Equal(-1000, entry.Points);
        Assert.Equal("Redeemed", entry.Action);
    }

    [Fact]
    public void Checkout_MorePointsOfferedThanTheOrderCosts_RedeemsOnlyWhatIsUsed()
    {
        // 1 x 10.00 + 8% tax = 10.80, so only 1080 of the offered 5000 points are usable.
        Fixture fixture = new(unitPrice: 10m, stock: 5, loyaltyPoints: 5000);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: 5000);

        Assert.True(result.Success);
        Assert.Equal(0m, fixture.ChargedAmount);
        Assert.Equal(3920, fixture.Customer.LoyaltyPoints);
    }

    [Fact]
    public void Checkout_MorePointsThanBalance_FailsWithoutChargingOrDebiting()
    {
        Fixture fixture = new(unitPrice: 100m, stock: 5, loyaltyPoints: 100);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: 500);

        Assert.False(result.Success);
        Assert.Equal("Not enough loyalty points for this redemption.", result.Message);
        Assert.Equal(100, fixture.Customer.LoyaltyPoints);
        Assert.Empty(fixture.Orders.Added);
    }

    [Fact]
    public void Checkout_NegativePoints_IsRejected()
    {
        Fixture fixture = new(unitPrice: 100m, stock: 5, loyaltyPoints: 500);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: -50);

        Assert.False(result.Success);
        Assert.Equal("Loyalty points cannot be negative.", result.Message);
        Assert.Equal(500, fixture.Customer.LoyaltyPoints);
    }

    [Fact]
    public void Checkout_OutOfStock_DoesNotTouchLoyaltyPoints()
    {
        Fixture fixture = new(unitPrice: 100m, stock: 0, loyaltyPoints: 500);

        CheckoutResult result = fixture.Checkout(loyaltyPoints: 500);

        Assert.False(result.Success);
        Assert.Equal("One or more items are out of stock.", result.Message);
        Assert.Equal(500, fixture.Customer.LoyaltyPoints);
    }

    /// <summary>Builds a checkout service over in-memory fakes for a single-line order.</summary>
    private sealed class Fixture
    {
        private const int TestProductId = 1;
        private const int TestCustomerId = 7;

        private readonly CheckoutService _checkout;
        private readonly Order _order;

        public Fixture(decimal unitPrice, int stock, int loyaltyPoints)
        {
            Customer = new Customer
            {
                Id = TestCustomerId,
                Name = "Ada",
                Email = "ada@example.com",
                LoyaltyPoints = loyaltyPoints,
            };

            Product product = new()
            {
                Id = TestProductId,
                Sku = "SKU-1",
                Name = "Widget",
                Price = unitPrice,
                StockQuantity = stock,
            };

            Orders = new FakeOrderRepository();
            History = new FakeLoyaltyPointsHistoryRepository();

            FakeProductRepository products = new(product);
            FakeCustomerRepository customers = new(Customer);

            _checkout = new CheckoutService(
                Orders,
                new InventoryService(products),
                new PricingService(),
                new PaymentService(),
                new NotificationService(),
                new DiscountService(new FakeDiscountRepository()),
                new LoyaltyService(customers, History));

            _order = new Order
            {
                Id = 42,
                CustomerId = TestCustomerId,
                Customer = Customer,
                Items = new List<OrderItem>
                {
                    new() { ProductId = TestProductId, Quantity = 1, UnitPrice = unitPrice },
                },
            };
        }

        public Customer Customer { get; }

        public FakeOrderRepository Orders { get; }

        public FakeLoyaltyPointsHistoryRepository History { get; }

        /// <summary>Amount actually taken for the order, or null if it was never paid.</summary>
        public decimal? ChargedAmount => _order.Payment?.Amount;

        public CheckoutResult Checkout(int? loyaltyPoints)
        {
            return _checkout.Checkout(_order, PaymentMethod.Card, discountCode: null, loyaltyPoints: loyaltyPoints);
        }
    }

    private sealed class FakeOrderRepository : IOrderRepository
    {
        public List<Order> Added { get; } = new();

        public Order? GetWithDetails(int orderId)
        {
            return Added.FirstOrDefault(o => o.Id == orderId);
        }

        public IReadOnlyList<Order> ForCustomer(int customerId)
        {
            return Added.Where(o => o.CustomerId == customerId).ToList();
        }

        public void Add(Order order)
        {
            Added.Add(order);
        }

        public void Save()
        {
        }
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product _product;

        public FakeProductRepository(Product product)
        {
            _product = product;
        }

        public Product? GetById(int id)
        {
            return _product.Id == id ? _product : null;
        }

        public IReadOnlyList<Product> InCategory(int categoryId)
        {
            return new List<Product>();
        }

        public void AdjustStock(int productId, int delta)
        {
            if (_product.Id == productId)
            {
                _product.StockQuantity += delta;
            }
        }

        public void Save()
        {
        }
    }

    private sealed class FakeCustomerRepository : ICustomerRepository
    {
        private readonly Customer _customer;

        public FakeCustomerRepository(Customer customer)
        {
            _customer = customer;
        }

        public Customer? GetWithOrders(int id)
        {
            return GetById(id);
        }

        public Customer? GetById(int id)
        {
            return _customer.Id == id ? _customer : null;
        }

        public void Add(Customer customer)
        {
            throw new NotSupportedException();
        }

        public void Save()
        {
        }
    }

    private sealed class FakeLoyaltyPointsHistoryRepository : ILoyaltyPointsHistoryRepository
    {
        public List<LoyaltyPointsHistory> Entries { get; } = new();

        public void Add(LoyaltyPointsHistory history)
        {
            Entries.Add(history);
        }

        public IReadOnlyList<LoyaltyPointsHistory> GetByCustomerId(int customerId)
        {
            return Entries.Where(h => h.CustomerId == customerId).ToList();
        }
    }

    private sealed class FakeDiscountRepository : IDiscountRepository
    {
        public DiscountCode? GetByCode(string code)
        {
            return null;
        }

        public void Add(DiscountCode discountCode)
        {
            throw new NotSupportedException();
        }

        public void Save()
        {
            throw new NotSupportedException();
        }
    }
}
