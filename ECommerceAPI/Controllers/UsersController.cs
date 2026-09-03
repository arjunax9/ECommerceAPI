using ECommerce.Data.Repositories;
using ECommerce.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    // Admin only: list users (no passwords)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        // repository doesn't expose GetAll; use context via repository methods is limited
        // For simplicity, use user repository via DB context access if available (not ideal)
        // We'll return single user by id for now or instruct to add a GetAll method if needed.
        return BadRequest(new { error = "GetAll not implemented on repository. Add IUserRepository.GetAllAsync to enable." });
    }

    // Admin only: update user role
    [HttpPut("{id}/role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] string role)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        user.Role = role ?? "Customer";
        await _userRepository.UpdateAsync(user);

        return NoContent();
    }

    // Get current user info (authenticated)
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        // Read 'sub' claim (JWT subject) which we set to the user id
        var subClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(subClaim, out var id))
        {
            return Ok(new { name = User.Identity?.Name, roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value) });
        }

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null) return NotFound();

        return Ok(new { user.Id, user.Name, user.Email, user.Role });
    }
}
