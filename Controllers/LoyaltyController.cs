using Microsoft.AspNetCore.Mvc;
using OrderShop.Data.Repositories;
using OrderShop.Services;

namespace OrderShop.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class LoyaltyController : ControllerBase
{
    private readonly LoyaltyService _loyaltyService;
    private readonly ILoyaltyPointsHistoryRepository _historyRepository;

    public LoyaltyController(
        LoyaltyService loyaltyService,
        ILoyaltyPointsHistoryRepository historyRepository)
    {
        _loyaltyService = loyaltyService;
        _historyRepository = historyRepository;
    }

    [HttpGet("{id:int}/loyalty-points")]
    public IActionResult GetLoyaltyPoints(int id)
    {
        int points = _loyaltyService.GetPointsBalance(id);
        return Ok(new { points });
    }

    [HttpGet("{id:int}/loyalty-points-history")]
    public IActionResult GetLoyaltyPointsHistory(int id)
    {
        var history = _historyRepository.GetByCustomerId(id);
        return Ok(history);
    }
}
