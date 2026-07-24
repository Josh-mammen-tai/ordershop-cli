using System.Collections.Generic;

namespace OrderShop.Models;

/// <summary>A single line in an order.</summary>
public sealed class OrderItem
{
    public OrderItem(string sku, string description, decimal unitPrice, int quantity)
    {
        Sku = sku;
        Description = description;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public string Sku { get; }

    public string Description { get; }

    public decimal UnitPrice { get; }

    public int Quantity { get; }

    /// <summary>Price for this line (unit price × quantity).</summary>
    public decimal LineTotal => UnitPrice * Quantity;
}

/// <summary>An order placed by a <see cref="Customer"/>.</summary>
public sealed class Order
{
    public Order(Customer customer)
    {
        Customer = customer;
        Items = new List<OrderItem>();
    }

    public Customer Customer { get; }

    public List<OrderItem> Items { get; }

    public void AddItem(OrderItem item) => Items.Add(item);
}
