using ECommerce.Business.Interfaces;
using ECommerce.Models.DTOs;
using ECommerce.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Register(User user)
    {
        try
        {
            // Prevent non-admins from creating Admin users. If the caller is anonymous or not Admin, do not allow assigning the Admin role.
            if (!string.IsNullOrWhiteSpace(user.Role) && user.Role.Equals("Admin", System.StringComparison.OrdinalIgnoreCase) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var created = await _authService.RegisterAsync(user);
            return Ok(new { created.Id, created.Name, created.Email });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var token = await _authService.LoginAsync(request.Email, request.Password);
        if (token == null)
            return Unauthorized();

        return Ok(new { token });
    }
}
