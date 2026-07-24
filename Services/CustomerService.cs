using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Customer-registration business flow: create a customer with a default
/// shipping address and send a welcome notification.
/// </summary>
public sealed class CustomerService
{
    private readonly ICustomerRepository _customers;
    private readonly NotificationService _notifications;

    public CustomerService(ICustomerRepository customers, NotificationService notifications)
    {
        _customers = customers;
        _notifications = notifications;
    }

    /// <summary>Register a new customer with an initial address.</summary>
    public Customer Register(string name, string email, Address address)
    {
        Customer customer = new()
        {
            Name = name,
            Email = email,
        };
        customer.Addresses.Add(address);

        _customers.Add(customer);
        _customers.Save();

        _notifications.Welcome(customer);
        return customer;
    }
}
