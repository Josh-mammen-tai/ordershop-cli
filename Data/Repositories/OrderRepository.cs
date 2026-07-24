using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

/// <summary>Data access for <see cref="Order"/> aggregates.</summary>
public interface IOrderRepository
{
    /// <summary>Load an order with every related entity eagerly included.</summary>
    Order? GetWithDetails(int orderId);

    /// <summary>All orders placed by a customer.</summary>
    IReadOnlyList<Order> ForCustomer(int customerId);

    void Add(Order order);

    void Save();
}

/// <summary>EF Core-backed <see cref="IOrderRepository"/>.</summary>
public sealed class OrderRepository : IOrderRepository
{
    private readonly ShopDbContext _db;

    public OrderRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Order? GetWithDetails(int orderId)
    {
        return _db.Orders
            .Include(o => o.Customer)
            .Include(o => o.ShippingAddress)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Payment)
            .Include(o => o.Shipment)
            .FirstOrDefault(o => o.Id == orderId);
    }

    public IReadOnlyList<Order> ForCustomer(int customerId)
    {
        return _db.Orders
            .Where(o => o.CustomerId == customerId)
            .ToList();
    }

    public void Add(Order order)
    {
        _db.Orders.Add(order);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
