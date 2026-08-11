using System.Collections.Generic;
using System.Linq;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

public interface ILoyaltyPointsHistoryRepository
{
    void Add(LoyaltyPointsHistory history);

    IReadOnlyList<LoyaltyPointsHistory> GetByCustomerId(int customerId);
}

public sealed class LoyaltyPointsHistoryRepository : ILoyaltyPointsHistoryRepository
{
    private readonly ShopDbContext _db;

    public LoyaltyPointsHistoryRepository(ShopDbContext db)
    {
        _db = db;
    }

    public void Add(LoyaltyPointsHistory history)
    {
        _db.LoyaltyPointsHistories.Add(history);
        _db.SaveChanges();
    }

    public IReadOnlyList<LoyaltyPointsHistory> GetByCustomerId(int customerId)
    {
        return _db.LoyaltyPointsHistories
            .Where(h => h.CustomerId == customerId)
            .ToList();
    }
}
