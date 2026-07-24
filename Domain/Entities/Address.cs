namespace OrderShop.Domain.Entities;

/// <summary>
/// A postal address. Many addresses belong to one <see cref="Customer"/>
/// (many-to-one), and an <see cref="Order"/> references one as its shipping address.
/// </summary>
public class Address
{
    public int Id { get; set; }

    public string Line1 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    /// <summary>Foreign key to the owning customer.</summary>
    public int CustomerId { get; set; }

    /// <summary>The customer this address belongs to (* → 1).</summary>
    public Customer Customer { get; set; } = null!;
}
