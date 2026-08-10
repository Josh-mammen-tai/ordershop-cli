using System;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Services;

/// <summary>
/// Product-review business flow: validate the product and rating, record the
/// review, and recompute the product's average rating.
/// </summary>
public sealed class ReviewService
{
    private const int MinRating = 1;
    private const int MaxRating = 5;

    private readonly IReviewRepository _reviews;
    private readonly IProductRepository _products;

    public ReviewService(IReviewRepository reviews, IProductRepository products)
    {
        _reviews = reviews;
        _products = products;
    }

    /// <summary>Submit a review for a product and return the updated average rating.</summary>
    public ReviewResult Submit(int productId, int customerId, int rating, string comment)
    {
        Product? product = _products.GetById(productId);
        if (product is null)
        {
            return new ReviewResult(false, 0d, "Product not found.");
        }

        if (rating < MinRating || rating > MaxRating)
        {
            return new ReviewResult(false, 0d, "Rating must be between 1 and 5.");
        }

        Review review = new()
        {
            ProductId = productId,
            CustomerId = customerId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow,
        };

        _reviews.Add(review);
        _reviews.Save();

        double average = _reviews.AverageRatingForProduct(productId);
        return new ReviewResult(true, average, "Review submitted.");
    }
}

/// <summary>The outcome of submitting a review.</summary>
public sealed class ReviewResult
{
    public ReviewResult(bool success, double averageRating, string message)
    {
        Success = success;
        AverageRating = averageRating;
        Message = message;
    }

    public bool Success { get; }

    public double AverageRating { get; }

    public string Message { get; }
}
