namespace OrderShop.Models;

/// <summary>A shop customer who can place orders.</summary>
public sealed class Customer
{
    public Customer(int id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public int Id { get; }

    public string Name { get; }

    public string Email { get; }
}
