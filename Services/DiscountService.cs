using System;
using System.Diagnostics.CodeAnalysis;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

public sealed class DiscountService
{
    private readonly IDiscountRepository _discountRepository;

    public DiscountService(IDiscountRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public DiscountCode? GetByCode(string code)
    {
        return _discountRepository.GetByCode(code);
    }

    public bool ValidateDiscountCode(string code, [NotNullWhen(true)] out DiscountCode? discountCode)
    {
        discountCode = GetByCode(code);
        if (discountCode is null)
        {
            return false;
        }

        DateTime now = DateTime.UtcNow;
        bool valid = discountCode.IsActive && now >= discountCode.ValidityStart && now <= discountCode.ValidityEnd;
        if (!valid)
        {
            // Don't hand back an unusable (inactive/expired) code to the caller.
            discountCode = null;
        }

        return valid;
    }

    public decimal ApplyDiscount(decimal orderTotal, DiscountCode discountCode)
    {
        decimal discountedTotal = discountCode.DiscountType == DiscountType.Percentage
            ? orderTotal * (1 - discountCode.DiscountValue / 100m)
            : orderTotal - discountCode.DiscountValue;

        return Math.Max(0m, discountedTotal);
    }
}
