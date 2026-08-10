using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrderShop.Domain.Entities;

namespace OrderShop.Data.Repositories;

/// <summary>Data access for <see cref="Product"/> catalog and stock.</summary>
public interface IProductRepository
{
    Product? GetById(int id);

    IReadOnlyList<Product> InCategory(int categoryId);

    /// <summary>Adjust on-hand stock by <paramref name="delta"/> (negative reserves stock).</summary>
    void AdjustStock(int productId, int delta);

    void Save();
}

/// <summary>EF Core-backed <see cref="IProductRepository"/>.</summary>
public sealed class ProductRepository : IProductRepository
{
    private readonly ShopDbContext _db;

    public ProductRepository(ShopDbContext db)
    {
        _db = db;
    }

    public Product? GetById(int id)
    {
        return _db.Products
            .Include(p => p.Category)
            .FirstOrDefault(p => p.Id == id);
    }

    public IReadOnlyList<Product> InCategory(int categoryId)
    {
        return _db.Products
            .Where(p => p.CategoryId == categoryId)
            .ToList();
    }

    public void AdjustStock(int productId, int delta)
    {
        Product? product = _db.Products.FirstOrDefault(p => p.Id == productId);
        if (product is not null)
        {
            product.StockQuantity += delta;
        }
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
