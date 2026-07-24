using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;
using OrderShop.Services;

namespace OrderShop.Controllers;

/// <summary>HTTP entry points for the customer-registration flow.</summary>
[ApiController]
[Route("api/customers")]
public sealed class CustomersController : ControllerBase
{
    private readonly CustomerService _customers;
    private readonly ICustomerRepository _repository;

    public CustomersController(CustomerService customers, ICustomerRepository repository)
    {
        _customers = customers;
        _repository = repository;
    }

    /// <summary>Register a new customer with a default shipping address.</summary>
    [HttpPost]
    public IActionResult Register([FromBody] RegisterCustomerRequest request)
    {
        Customer customer = _customers.Register(request.Name, request.Email, request.Address);
        return Ok(new { customer.Id });
    }

    /// <summary>Fetch a customer with their orders and addresses.</summary>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        Customer? customer = _repository.GetWithOrders(id);
        return customer is null ? NotFound() : Ok(customer);
    }
}

/// <summary>Request body for registering a customer.</summary>
public sealed class RegisterCustomerRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Address Address { get; set; } = new();
}
