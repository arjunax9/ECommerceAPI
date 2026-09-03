using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ECommerceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DebugController : ControllerBase
{
    // Returns raw Authorization header and the current user's claims for troubleshooting
    [HttpGet("whoami")]
    [Authorize]
    public IActionResult WhoAmI()
    {
        var authHeader = Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

        var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();

        return Ok(new { authorization = authHeader, claims });
    }
}
