using ECommerce.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _service;

    public InventoryController(IInventoryService service)
    {
        _service = service;
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetQuantity(int productId)
    {
        var qty = await _service.GetQuantityAsync(productId);
        return Ok(new { productId, quantity = qty });
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateQuantity(int productId, [FromBody] int quantity)
    {
        await _service.UpdateQuantityAsync(productId, quantity);
        return NoContent();
    }
}
