using ECommerce.Business.Interfaces;
using ECommerce.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        try
        {
            var order = await _service.CreateOrderAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException ex)
        {
            // return a 400 with the underlying reason instead of a 500
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var orders = await _service.GetAllAsync();
        return Ok(orders);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _service.GetByIdAsync(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost("{id}/checkout")]
    public async Task<IActionResult> Checkout(int id)
    {
        await _service.CheckoutAsync(id);
        return Ok(new { message = "Order checked out" });
    }

    [HttpPost("{id}/payment")]
    public async Task<IActionResult> Pay(int id)
    {
        var ok = await _service.ProcessPaymentAsync(id);
        if (!ok) return BadRequest(new { message = "Payment failed" });
        return Ok(new { message = "Payment successful" });
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var order = await _service.ApproveOrderAsync(id);
            return Ok(new { message = "Order approved successfully", order });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // New helper to get orders for a user
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var orders = await _service.GetByUserIdAsync(userId);
        return Ok(orders);
    }
}
