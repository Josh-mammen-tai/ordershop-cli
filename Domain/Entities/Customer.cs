using System.Collections.Generic;

namespace OrderShop.Domain.Entities;

/// <summary>
/// A customer who places orders. One customer has many <see cref="Order"/>s and
/// many <see cref="Address"/>es (one-to-many on both sides).
/// </summary>
public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int LoyaltyPoints { get; set; }

    /// <summary>Orders placed by this customer (1 → *).</summary>
    public ICollection<Order> Orders { get; set; } = new List<Order>();

    /// <summary>Addresses belonging to this customer (1 → *).</summary>
    public ICollection<Address> Addresses { get; set; } = new List<Address>();

    /// <summary>History of loyalty points earned and redeemed.</summary>
    public ICollection<LoyaltyPointsHistory> LoyaltyPointsHistories { get; set; } = new List<LoyaltyPointsHistory>();
}
