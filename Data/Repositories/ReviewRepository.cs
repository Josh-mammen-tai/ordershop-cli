using System.Collections.Generic;
using System.Linq;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

/// <summary>Data access for <see cref="Review"/> entities.</summary>
public interface IReviewRepository
{
    void Add(Review review);

    IReadOnlyList<Review> ForProduct(int productId);

    /// <summary>Average star rating across a product's reviews (0 when none).</summary>
    double AverageRatingForProduct(int productId);

    void Save();
}

/// <summary>EF Core-backed <see cref="IReviewRepository"/>.</summary>
public sealed class ReviewRepository : IReviewRepository
{
    private readonly ShopDbContext _db;

    public ReviewRepository(ShopDbContext db)
    {
        _db = db;
    }

    public void Add(Review review)
    {
        _db.Reviews.Add(review);
    }

    public IReadOnlyList<Review> ForProduct(int productId)
    {
        return _db.Reviews
            .Where(r => r.ProductId == productId)
            .ToList();
    }

    public double AverageRatingForProduct(int productId)
    {
        List<int> ratings = _db.Reviews
            .Where(r => r.ProductId == productId)
            .Select(r => r.Rating)
            .ToList();

        return ratings.Count == 0 ? 0d : ratings.Average();
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
