using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Domain.Entities;
using OrderShop.Services;

namespace OrderShop.Controllers;

/// <summary>HTTP entry points for the product catalog and the restock flow.</summary>
[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _products;
    private readonly InventoryService _inventory;

    public ProductsController(IProductRepository products, InventoryService inventory)
    {
        _products = products;
        _inventory = inventory;
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

    /// <summary>Add stock for a product (restock flow).</summary>
    [HttpPost("{id:int}/restock")]
    public IActionResult Restock(int id, [FromQuery] int quantity)
    {
        _inventory.Restock(id, quantity);
        return Ok();
    }
}
