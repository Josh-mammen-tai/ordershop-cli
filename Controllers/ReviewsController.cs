using Microsoft.AspNetCore.Mvc;
using OrderShop.Services;

namespace OrderShop.Controllers;

/// <summary>HTTP entry point for the product-review flow.</summary>
[ApiController]
[Route("api/products/{productId:int}/reviews")]
public sealed class ReviewsController : ControllerBase
{
    private readonly ReviewService _reviews;

    public ReviewsController(ReviewService reviews)
    {
        _reviews = reviews;
    }

    /// <summary>Submit a product review and get the updated average rating.</summary>
    [HttpPost]
    public IActionResult Submit(int productId, [FromBody] SubmitReviewRequest request)
    {
        ReviewResult result = _reviews.Submit(productId, request.CustomerId, request.Rating, request.Comment);
        return result.Success
            ? Ok(new { averageRating = result.AverageRating })
            : BadRequest(result.Message);
    }
}

/// <summary>Request body for submitting a review.</summary>
public sealed class SubmitReviewRequest
{
    public int CustomerId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;
}
