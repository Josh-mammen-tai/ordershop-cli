using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;

namespace OrderShop.Controllers;

/// <summary>HTTP entry points for browsing the product catalog.</summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;

    public ProductsController(IProductRepository products)
    {
        _products = products;
    }

    /// <summary>Fetch a single product with its category.</summary>
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        Product? product = _products.Get(id);
        return product is null ? NotFound() : Ok(product);
    }

    /// <summary>List every product in a category.</summary>
    [HttpGet("category/{categoryId:int}")]
    public IActionResult InCategory(int categoryId)
    {
        return Ok(_products.InCategory(categoryId));
    }
}
