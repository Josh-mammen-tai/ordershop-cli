using System.Linq;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

public interface IDiscountRepository
{
    DiscountCode? GetByCode(string code);

    void Add(DiscountCode discountCode);

    void Save();
}

public sealed class DiscountRepository : IDiscountRepository
{
    private readonly ShopDbContext _db;

    public DiscountRepository(ShopDbContext db)
    {
        _db = db;
    }

    public DiscountCode? GetByCode(string code)
    {
        return _db.DiscountCodes
            .FirstOrDefault(dc => dc.Code == code);
    }

    public void Add(DiscountCode discountCode)
    {
        _db.DiscountCodes.Add(discountCode);
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
