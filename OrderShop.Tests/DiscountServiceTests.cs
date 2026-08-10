using System;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;
using OrderShop.Services;
using Xunit;

namespace OrderShop.Tests;

public sealed class DiscountServiceTests
{
    [Fact]
    public void ValidateDiscountCode_ValidCode_ReturnsTrue()
    {
        DiscountCode discount = new()
        {
            Code = "VALID",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 20m,
            ValidityStart = DateTime.UtcNow.AddDays(-1),
            ValidityEnd = DateTime.UtcNow.AddDays(1),
            IsActive = true,
        };

        DiscountService service = new(new FakeDiscountRepository(discount));

        bool valid = service.ValidateDiscountCode("VALID", out DiscountCode? found);

        Assert.True(valid);
        Assert.Same(discount, found);
    }

    [Fact]
    public void ApplyDiscount_PercentageDiscount_ReducesTotal()
    {
        DiscountCode discount = new()
        {
            DiscountType = DiscountType.Percentage,
            DiscountValue = 25m,
        };

        DiscountService service = new(new FakeDiscountRepository());

        decimal total = service.ApplyDiscount(100m, discount);

        Assert.Equal(75m, total);
    }

    [Fact]
    public void ApplyDiscount_FixedAmountDiscount_ReducesTotal()
    {
        DiscountCode discount = new()
        {
            DiscountType = DiscountType.FixedAmount,
            DiscountValue = 15m,
        };

        DiscountService service = new(new FakeDiscountRepository());

        decimal total = service.ApplyDiscount(100m, discount);

        Assert.Equal(85m, total);
    }

    [Fact]
    public void ValidateDiscountCode_ExpiredCode_ReturnsFalse()
    {
        DiscountCode discount = new()
        {
            Code = "EXPIRED",
            DiscountType = DiscountType.FixedAmount,
            DiscountValue = 10m,
            ValidityStart = DateTime.UtcNow.AddDays(-10),
            ValidityEnd = DateTime.UtcNow.AddDays(-1),
            IsActive = true,
        };

        DiscountService service = new(new FakeDiscountRepository(discount));

        bool valid = service.ValidateDiscountCode("EXPIRED", out DiscountCode? found);

        Assert.False(valid);
        Assert.Null(found);
    }

    private sealed class FakeDiscountRepository : IDiscountRepository
    {
        private readonly DiscountCode? _discount;

        public FakeDiscountRepository(DiscountCode? discount = null)
        {
            _discount = discount;
        }

        public DiscountCode? GetByCode(string code)
        {
            return _discount is not null && string.Equals(_discount.Code, code, StringComparison.Ordinal)
                ? _discount
                : null;
        }

        public void Add(DiscountCode discountCode)
        {
            throw new NotSupportedException();
        }

        public void Save()
        {
            throw new NotSupportedException();
        }
    }
}
