using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

/// <summary>Data access for <see cref="Customer"/> aggregates.</summary>
public interface ICustomerRepository
{
    /// <summary>Load a customer with their orders and addresses eagerly included.</summary>
    Customer? GetWithOrders(int id);

    void Add(Customer customer);

    void Save();
}

/// <summary>EF Core-backed <see cref="ICustomerRepository"/>.</summary>
public sealed class CustomerRepository : ICustomerRepository
{
    private readonly ShopDbContext _db;

    public CustomerRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Customer? GetWithOrders(int id)
    {
        return _db.Customers
            .Include(c => c.Orders)
            .Include(c => c.Addresses)
            .FirstOrDefault(c => c.Id == id);
    }

    public void Add(Customer customer)
    {
        _db.Customers.Add(customer);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
