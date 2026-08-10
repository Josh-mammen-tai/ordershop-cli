using System;

namespace OrderShop.Domain.Entities;

public class DiscountCode
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public DiscountType DiscountType { get; set; }

    public decimal DiscountValue { get; set; }

    public DateTime ValidityStart { get; set; }

    public DateTime ValidityEnd { get; set; }

    public bool IsActive { get; set; }
}

public enum DiscountType
{
    Percentage,
    FixedAmount,
}
