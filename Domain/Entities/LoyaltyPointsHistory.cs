using System;

namespace OrderShop.Domain.Entities;

public class LoyaltyPointsHistory
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int Points { get; set; }

    public string Action { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public Customer Customer { get; set; } = null!;
}
